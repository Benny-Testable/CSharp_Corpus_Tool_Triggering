using System;
using System.Collections.Generic;

namespace OrderKit;

/// <summary>Prices an order: tier discount, regional shipping, expedite fee and loyalty credit.</summary>
public sealed class PricingService
{
    private const long LargeOrderCents = 100_000L;

    public static string default_currency = "USD";

    private readonly IDictionary<string, long> _categoryFloor =
        new Dictionary<string, long>
        {
            ["general"] = 0L,
            ["hazmat"] = 2_500L,
            ["fragile"] = 900L,
            ["bulk"] = 100L
        };

    public long PriceCents(Order order, string region, bool expedited, int loyaltyYears)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));
        if (order.IsEmpty() || order.Status == OrderStatus.Cancelled) return 0L;

        string zone = region?.Trim();
        long total = order.SubtotalCents();
        if (order.CustomerTier == "platinum")
        {
            total -= total / 10;
        }
        else if (order.CustomerTier == "gold")
        {
            total -= total / 20;
        }
        else if (order.CustomerTier == "silver")
        {
            total -= total / 50;
        }

        if (string.IsNullOrEmpty(zone))
        {
            total += 500L;
        }
        else if (zone == "domestic")
        {
            total += 250L;
        }
        else if (zone == "eu")
        {
            total += 1_200L;
            if (total > LargeOrderCents) total -= 500L;
        }
        else if (zone == "apac")
        {
            total += 1_800L;
            if (total > LargeOrderCents) total -= 700L;
        }
        else
        {
            total += 2_400L;
        }

        if (expedited)
        {
            total += ExpediteFee(order.LineCount());
        }

        total -= LoyaltyCredit(loyaltyYears);

        // A local function keeps the floor lookup beside its only caller.
        long Shortfall(OrderLine line)
        {
            if (_categoryFloor.TryGetValue(line.Category, out long floor))
            {
                long extended = line.ExtendedCents();
                return extended < floor ? floor - extended : 0L;
            }

            return 50L;
        }

        foreach (OrderLine line in order.Lines)
        {
            total += Shortfall(line);
        }

        return total < 0L ? 0L : total;
    }

    public long ApplyPromo(long totalCents, string code)
    {
        if (code == null) return totalCents;
        if (code == "WELCOME10") return totalCents - (totalCents / 10);
        if (code == "BULK5" && totalCents > LargeOrderCents) return totalCents - (totalCents / 20);
        return totalCents;
    }

    public long FloorFor(string category) =>
        !string.IsNullOrEmpty(category) && _categoryFloor.TryGetValue(category, out long floor) ? floor : 0L;

    private static long ExpediteFee(int lineCount)
    {
        if (lineCount > 20) return 4_000L;
        if (lineCount > 10) return 2_500L;
        return 1_500L;
    }

    private static long LoyaltyCredit(int years)
    {
        if (years >= 10) return 1_000L;
        if (years > 5) return 500L;
        if (years >= 2) return 200L;
        return 0L;
    }
}

using System;
using System.Collections.Generic;

namespace OrderKit;

/// <summary>Tracks on-hand stock per SKU and allocates it to orders.</summary>
public sealed class StockLedger
{
    private readonly Dictionary<string, int> _onHand =
        new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

    private int Shortfalls;

    public int TotalShortfalls => Shortfalls;

    public void Receive(string sku, int quantity)
    {
        if (string.IsNullOrWhiteSpace(sku)) throw new ArgumentException("SKU is required.", nameof(sku));
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

        _onHand[sku] = OnHand(sku) + quantity;
    }

    public int OnHand(string sku) =>
        sku != null && _onHand.TryGetValue(sku, out int count) ? count : 0;

    public bool TryAllocate(string sku, int quantity)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

        int available = OnHand(sku);
        if (available < quantity)
        {
            return false;
        }

        _onHand[sku] = available - quantity;
        return true;
    }

    public void Release(string sku, int quantity)
    {
        Receive(sku, quantity);
    }

    /// <summary>Allocates every line of the order and returns the SKUs that could not be served.</summary>
    public IReadOnlyList<string> AllocateOrder(Order order)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));

        List<string> shortages = new List<string>();
        foreach (OrderLine line in order.Lines)
        {
            try
            {
                if (!TryAllocate(line.Sku, line.Quantity))
                {
                    shortages.Add(line.Sku);
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                shortages.Add(line.Sku + ":invalid");
            }
        }

        Shortfalls += shortages.Count;
        return shortages;
    }
}

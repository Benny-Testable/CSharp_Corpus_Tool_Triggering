using System;

namespace OrderKit;

public enum OrderStatus
{
    Draft = 0,
    Submitted = 1,
    Priced = 2,
    Fulfilled = 3,
    Cancelled = 4,
    Rejected = 5
}

public static class OrderStatusExtensions
{
    public static bool IsTerminal(this OrderStatus status) =>
        status == OrderStatus.Fulfilled
        || status == OrderStatus.Cancelled
        || status == OrderStatus.Rejected;

    public static string Label(this OrderStatus status) => status switch
    {
        OrderStatus.Draft => "draft",
        OrderStatus.Submitted => "submitted",
        OrderStatus.Priced => "priced",
        OrderStatus.Fulfilled => "fulfilled",
        OrderStatus.Cancelled => "cancelled",
        OrderStatus.Rejected => "rejected",
        _ => "unknown"
    };

    public static bool CanMoveTo(this OrderStatus current, OrderStatus next)
    {
        switch (current)
        {
            case OrderStatus.Draft:
                return next == OrderStatus.Submitted || next == OrderStatus.Cancelled;
            case OrderStatus.Submitted:
                return next == OrderStatus.Priced
                    || next == OrderStatus.Rejected
                    || next == OrderStatus.Cancelled;
            case OrderStatus.Priced:
                return next == OrderStatus.Fulfilled || next == OrderStatus.Cancelled;
            default:
                return false;
        }
    }

    public static bool TryParseLabel(string label, out OrderStatus status)
    {
        if (!string.IsNullOrWhiteSpace(label)
            && Enum.TryParse(label.Trim(), true, out OrderStatus parsed)
            && Enum.IsDefined(typeof(OrderStatus), parsed))
        {
            status = parsed;
            return true;
        }

        status = OrderStatus.Draft;
        return false;
    }
}

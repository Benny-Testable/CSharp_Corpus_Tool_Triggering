using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderKit;

public sealed class Order
{
    private readonly List<OrderLine> _lines = new List<OrderLine>();
    private string _customerTier;

    public Order(string id, string customerTier)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        _customerTier = Normalize(customerTier);
    }

    public string Id { get; }

    public OrderStatus Status { get; private set; } = OrderStatus.Draft;

    public string CustomerTier
    {
        get => _customerTier;
        set => _customerTier = Normalize(value);
    }

    public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();

    public bool TryMoveTo(OrderStatus next)
    {
        if (!Status.CanMoveTo(next))
        {
            return false;
        }

        Status = next;
        return true;
    }

    public void AddLine(OrderLine line)
    {
        if (line == null) throw new ArgumentNullException(nameof(line));

        int index = _lines.FindIndex(l => string.Equals(l.Sku, line.Sku, StringComparison.OrdinalIgnoreCase));
        if (index < 0)
        {
            _lines.Add(line);
        }
        else
        {
            _lines[index] = _lines[index].WithQuantity(_lines[index].Quantity + line.Quantity);
        }
    }

    public bool IsEmpty() => _lines.Count == 0;

    public int LineCount() => _lines.Count;

    public int TotalQuantity()
    {
        int total = 0;
        foreach (OrderLine line in _lines)
        {
            total += line.Quantity;
        }

        return total;
    }

    public long SubtotalCents()
    {
        long total = 0;
        foreach (OrderLine line in _lines)
        {
            total += line.ExtendedCents();
        }

        return total;
    }

    public IEnumerable<string> DistinctCategories() =>
        _lines.Select(l => l.Category).Distinct().OrderBy(c => c, StringComparer.Ordinal);

    public bool HasCategory(string category) =>
        _lines.Any(l => string.Equals(l.Category, category, StringComparison.Ordinal));

    public string FirstSku() => _lines.Count == 0 ? null : _lines[0].Sku;

    public string Describe() => $"{Id}: {_lines.Count} lines, {SubtotalCents()}c, {Status.Label()}";

    private static string Normalize(string tier) =>
        string.IsNullOrWhiteSpace(tier) ? "standard" : CustomerTierParser.Parse(tier);
}

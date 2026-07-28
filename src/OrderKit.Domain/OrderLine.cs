using System;

namespace OrderKit;

/// <summary>A single priced line on an order. Immutable.</summary>
public sealed class OrderLine
{
    public OrderLine(string sku, int quantity, long unitCents, string category)
    {
        Sku = sku ?? throw new ArgumentNullException(nameof(sku));
        Quantity = quantity >= 0
            ? quantity
            : throw new ArgumentOutOfRangeException(nameof(quantity));
        UnitCents = unitCents >= 0
            ? unitCents
            : throw new ArgumentOutOfRangeException(nameof(unitCents));
        Category = string.IsNullOrWhiteSpace(category) ? "general" : category;
    }

    public string Sku { get; }
    public int Quantity { get; }
    public long UnitCents { get; }
    public string Category { get; }

    public long ExtendedCents() => UnitCents * Quantity;

    public OrderLine WithQuantity(int quantity) => new OrderLine(Sku, quantity, UnitCents, Category);

    public override string ToString() => $"{Sku} x{Quantity} @ {UnitCents}";
}

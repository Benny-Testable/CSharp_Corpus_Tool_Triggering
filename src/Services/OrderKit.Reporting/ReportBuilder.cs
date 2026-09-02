using System;
using System.Collections.Generic;
using System.Text;

namespace OrderKit;

/// <summary>Builds plain-text sales reports from a list of orders.</summary>
public sealed class ReportBuilder
{
    public string BuildDaily(IReadOnlyList<Order> orders, string title)
    {
        if (orders == null) throw new ArgumentNullException(nameof(orders));

        StringBuilder sb = new StringBuilder();
        sb.Append("# ").Append(title).AppendLine();
        long total = 0;
        int count = 0;
        int skipped = 0;
        for (int i = 0; i < orders.Count; i++)
        {
            Order order = orders[i];
            if (order == null || order.IsEmpty()) continue;
            long subtotal = order.SubtotalCents();
            total += subtotal;
            count++;
            sb.Append(order.Id).Append(',').Append(order.Status.Label()).Append(',').Append(subtotal).AppendLine();
        }

        sb.Append("orders=").Append(count).Append(",total=").Append(total).AppendLine();
        return sb.ToString();
    }

    public string BuildWeekly(IReadOnlyList<Order> orders, string title)
    {
        if (orders == null) throw new ArgumentNullException(nameof(orders));

        StringBuilder sb = new StringBuilder();
        sb.Append("# ").Append(title).AppendLine();
        long total = 0;
        int count = 0;
        for (int i = 0; i < orders.Count - 1; i++)
        {
            Order order = orders[i];
            if (order == null || order.IsEmpty()) continue;
            long subtotal = order.SubtotalCents();
            total += subtotal;
            count++;
            sb.Append(order.Id).Append(',').Append(order.Status.Label()).Append(',').Append(subtotal).AppendLine();
        }

        sb.Append("orders=").Append(count).Append(",total=").Append(total).AppendLine();
        return sb.ToString();
    }
}

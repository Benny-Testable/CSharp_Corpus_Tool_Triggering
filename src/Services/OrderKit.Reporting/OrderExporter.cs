using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OrderKit;

public sealed class OrderLineDto
{
    public string Sku { get; set; }
    public int Quantity { get; set; }
    public long UnitCents { get; set; }
    public string Category { get; set; }
}

public sealed class OrderDto
{
    public string Id { get; set; }
    public string Tier { get; set; }
    public string Status { get; set; }
    public List<OrderLineDto> Lines { get; set; } = new List<OrderLineDto>();
}

/// <summary>Converts orders to and from the JSON documents exchanged with partner systems.</summary>
public static class OrderExporter
{
    public static OrderDto ToDto(Order order)
    {
        OrderDto dto = new OrderDto
        {
            Id = order.Id,
            Tier = order.CustomerTier,
            Status = order.Status.Label()
        };

        foreach (OrderLine line in order.Lines)
        {
            dto.Lines.Add(new OrderLineDto
            {
                Sku = line.Sku,
                Quantity = line.Quantity,
                UnitCents = line.UnitCents,
                Category = line.Category
            });
        }

        return dto;
    }

    public static string ToJson(Order order) =>
        JsonConvert.SerializeObject(ToDto(order), Formatting.Indented);

    public static string ToCompactJson(Order order) =>
        System.Text.Json.JsonSerializer.Serialize(ToDto(order));

    public static OrderDto FromJson(string json)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        return JsonConvert.DeserializeObject<OrderDto>(json, settings);
    }

    public static bool TryFromJson(string json, out OrderDto dto)
    {
        try
        {
            dto = FromJson(json);
            return dto != null;
        }
        catch (Exception)
        {
            dto = null;
            return false;
        }
    }
}

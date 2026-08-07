using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderKit;

/// <summary>Async facade over <see cref="PricingService"/> for callers on a request pipeline.</summary>
public sealed class AsyncPricingGateway
{
    private readonly PricingService _pricing;

    public AsyncPricingGateway(PricingService pricing)
    {
        _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
    }

    public async Task<long> PriceAsync(Order order, string region)
    {
        await Task.Delay(1).ConfigureAwait(false);
        return _pricing.PriceCents(order, region, false, 0);
    }

    public async Task<long> PriceBatchAsync(IEnumerable<Order> orders, string region)
    {
        long total = 0;
        foreach (Order order in orders)
        {
            total += await PriceAsync(order, region).ConfigureAwait(false);
        }

        return total;
    }
}

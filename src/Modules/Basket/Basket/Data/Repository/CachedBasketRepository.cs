using Basket.Data.JsonConverter;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.Data.Repository;

// this class act as proxy and decorator pattern
public class CachedBasketRepository(IBasketRepository repository, IDistributedCache cache)
    : IBasketRepository
{
    private readonly JsonSerializerOptions _option = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        Converters = { new ShoppingCartConverter(), new ShoppingCartItemConverter() }
    };


    public async Task<ShoppingCart> GetBasket(string userName, bool asNoTracking = true, CancellationToken cancellationToken = default)
    {
        if (!asNoTracking)
        {
            return await repository.GetBasket(userName, false, cancellationToken);
        }


        var cachedBasket = await cache.GetStringAsync(userName, cancellationToken);
        if (!string.IsNullOrEmpty(cachedBasket))
        {
            // DeSerializer 

            return JsonSerializer.Deserialize<ShoppingCart>(cachedBasket,_option)!;
        }
        var basket = await repository.GetBasket(userName, asNoTracking, cancellationToken);

        // Serialize
        await cache.SetStringAsync(userName, JsonSerializer.Serialize(basket,_option), cancellationToken);

        return basket;

    }

    public async Task<ShoppingCart> CreateBasket(ShoppingCart basket, CancellationToken cancellationToken = default)
    {
        await repository.CreateBasket(basket, cancellationToken);
        await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket,_option), cancellationToken);
        return basket;
    }

    public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
    {
        await repository.DeleteBasket(userName, cancellationToken);
        await cache.RemoveAsync(userName, cancellationToken);
        return true;
    }


    public async Task<int> SaveChangesAsync(string? userName = null, CancellationToken cancellationToken = default)
    {
        var result = await repository.SaveChangesAsync(userName, cancellationToken);

        if (userName is not null)
        {
            await cache.RemoveAsync(userName, cancellationToken);
        }

        return result;
        // Review
    }
}

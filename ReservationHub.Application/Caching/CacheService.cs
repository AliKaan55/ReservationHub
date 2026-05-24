using System.Text.Json;
using StackExchange.Redis;

namespace ReservationHub.Application.Caching;

public class CacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public CacheService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _db = redis.GetDatabase();
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(key, json, expiry ?? TimeSpan.FromMinutes(10));
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await _db.StringGetAsync(key);
        if (json.IsNullOrEmpty) return default;
        return JsonSerializer.Deserialize<T>(json!);
    }

    public async Task RemoveAsync(string key)
        => await _db.KeyDeleteAsync(key);

    public async Task RemoveByPrefixAsync(string prefix)
    {
        var endPoint = _redis.GetEndPoints().First();
        var server = _redis.GetServer(endPoint);

        var keys = server.Keys(pattern: $"*{prefix}*").ToArray();

        foreach (var key in keys)
            await _db.KeyDeleteAsync(key);
    }
}
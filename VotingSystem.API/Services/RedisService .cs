using StackExchange.Redis;
using System.Net;

namespace VotingSystem.API.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _db;
        public RedisService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task<string?> GetCachedValueAsync(string key)
        {
            var value = await _db.StringGetAsync(key);
            return value.HasValue ? value.ToString() : null;
        }

        public async Task SetCachedValueAsync(string key, string value, TimeSpan? expiration = null)
        {
            if (expiration.HasValue)
            {
                await _db.StringSetAsync(key, value, expiration.Value);
            }
            else
            {
                await _db.StringSetAsync(key, value);
            }
        }

        public async Task RemoveAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }

        public async Task ClearAllAsync()
        {
            var endpoints = _db.Multiplexer.GetEndPoints();
            foreach (var endpoint in endpoints)
            {
                var server = _db.Multiplexer.GetServer(endpoint);
                foreach (var key in server.Keys())
                {
                    await _db.KeyDeleteAsync(key);
                }
            }
        }
    }
}

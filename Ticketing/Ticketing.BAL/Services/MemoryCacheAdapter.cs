using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;
using Ticketing.BAL.Contracts;

namespace Ticketing.BAL.Services
{
    public class CachedObject
    {
        public object value;
        public CancellationTokenSource tokenSource;
    }

    public class MemoryCacheAdapter : ICacheAdapter
    {
        ConcurrentDictionary<string, CachedObject> cacheVlues = new ConcurrentDictionary<string, CachedObject>(StringComparer.OrdinalIgnoreCase);

        private readonly IMemoryCache _cache;
        IConfiguration _configuration;

        private MemoryCacheEntryOptions _options;

        public MemoryCacheAdapter(IMemoryCache cache, IConfiguration configuration)
        {
            _cache = cache;
            _configuration = configuration;

            var keySliding = _configuration["MemoryCacheAdapter:SetSlidingExpiration"];
            var keyAbsolute = _configuration["MemoryCacheAdapter:SetAbsoluteExpiration"];

            int.TryParse(keySliding, out int sliding);
            int.TryParse(keyAbsolute, out int absolute);

            _options = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(sliding))
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(absolute));
        }

        public T Get<T>(string key)
        {
            if (key is null)
            {
                return default(T);
            }

            if (_cache.TryGetValue<T>(key, out var value))
            {
                return value;
            }
            return default(T);
        }

        public void Set<T>(string key, T value)
        {
            if (key is null || string.IsNullOrWhiteSpace(key) || value is null)
            {
                return;
            }

            var cts = new CancellationTokenSource();
            var pause = new ManualResetEvent(false);
            _options
            .AddExpirationToken(new CancellationChangeToken(cts.Token))
            .RegisterPostEvictionCallback(
            (key, v, reason, substate) =>
            {
                pause.Set();
                cacheVlues.TryRemove((string)key, out var outValue);
            });
            _cache.Set(key, value, _options);

            var cachedObject = new CachedObject();
            cachedObject.value = value;
            cachedObject.tokenSource = cts;

            cacheVlues.TryAdd(key, cachedObject);
        }

        public void Remove(string key)
        {
            if (cacheVlues.TryGetValue(key, out CachedObject cachedObject))
            {
                cachedObject.tokenSource.Cancel();
                cacheVlues.TryRemove(key, out var outValue);
            };

            _cache.Remove(key);
        }

        public void Invalidate(string subKey)
        {
            foreach (var pair in cacheVlues)
            {
                if (pair.Key.Contains(subKey))
                {
                    var value = pair.Value;
                    value.tokenSource.Cancel();
                    cacheVlues.TryRemove(pair.Key, out var outValue);
                }
            }
        }
    }
}

using System;
using System.Runtime.Caching;

namespace Gyldendal.Jini.Services.Common.Utils
{
    public static class MemoryCacher
    {
        static readonly MemoryCache MemoryCache;

        static MemoryCacher()
        {
            MemoryCache = MemoryCache.Default;
        }

        private static DateTimeOffset ExpiryTime => DateTimeOffset.UtcNow.AddHours(24);

        public static object GetValue(string key)
        {
            var value = MemoryCache.Get(key);

            if (value != null)
            {
                Add(key, value);
            }
            return value;
        }

        public static void Add(string key, object item)
        {
            var cacheItemPolicy = new CacheItemPolicy { AbsoluteExpiration = ExpiryTime };
            var cacheItem = new CacheItem(key, item);
            if (!MemoryCache.Contains(key))
            {
                MemoryCache.Add(cacheItem, cacheItemPolicy);
            }
            else
            {
                MemoryCache.Set(cacheItem, cacheItemPolicy);
            }
        }
        public static void Delete(string key)
        {
            if (MemoryCache.Contains(key))
            {
                MemoryCache.Remove(key);
            }
        }
    }
}
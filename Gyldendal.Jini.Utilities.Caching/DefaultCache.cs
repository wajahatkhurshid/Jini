using Gyldendal.Jini.Utilities.Caching.Interfaces;
using System;
using RuntimeCaching = System.Runtime.Caching;

namespace Gyldendal.Jini.Utilities.Caching
{
    public class DefaultCache : ICacher
    {
        private readonly RuntimeCaching.MemoryCache _memoryCache;

        public DefaultCache()
        {
            _memoryCache = RuntimeCaching.MemoryCache.Default;
        }

        public bool Add<T>(string key, T value, TimeSpan absExpiration)
        {
            if (_memoryCache.Contains(key))
                _memoryCache.Remove(key);

            return _memoryCache.Add(key, value,
                new RuntimeCaching.CacheItemPolicy { AbsoluteExpiration = DateTime.Now.Add(absExpiration) });
        }

        public bool Delete<T>(string key, T value)
        {
            if (_memoryCache.Contains(key))
            {
                _memoryCache.Remove(key);
                return true;
            }
            return false;
        }

        public T GetValue<T>(string key)
        {
            if (_memoryCache.Contains(key))
            {
                var val = _memoryCache.Get(key);
                if (val is T)
                    return (T)val;
            }
            return default(T);
        }
    }
}
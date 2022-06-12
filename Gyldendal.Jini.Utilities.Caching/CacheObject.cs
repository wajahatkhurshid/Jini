using System;

namespace Gyldendal.Jini.Utilities.Caching
{
    public class CacheObject<T>
    {
        public CacheObject(string key, DateTime expirationTime, T cacheObject)
        {
            Key = key;
            ExpirationTime = expirationTime;
            ObjectValue = cacheObject;
        }

        public CacheObject()
        {
        }

        public string Key { get; }

        public DateTime ExpirationTime { get; }

        public T ObjectValue { get; }
    }
}
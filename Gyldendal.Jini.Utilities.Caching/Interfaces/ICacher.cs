using System;

namespace Gyldendal.Jini.Utilities.Caching.Interfaces
{
    public interface ICacher
    {
        T GetValue<T>(string key);

        bool Add<T>(string key, T value, TimeSpan absExpiration);

        //bool Add(string key, object value, TimeSpan absExpiration);

        bool Delete<T>(string key, T value);

        //bool Delete(string key);
    }
}
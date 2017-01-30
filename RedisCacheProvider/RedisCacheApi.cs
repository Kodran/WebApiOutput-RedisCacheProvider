using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisCacheProvider
{
    public class RedisCacheApi : IRedisCacheApi
    {
        private static string _YOUR_KEY = "Primary connection string for StackExchange.Redis";

        private static IDatabase _redisCache =
           ConnectionMultiplexer.Connect(_YOUR_KEY)
           .GetDatabase();

        public void Add(string key, string value, int expires = 30)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
            {
                throw new ArgumentException();
            }
            _redisCache.StringSet(key, value, TimeSpan.FromSeconds(expires));
        }

        public string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException();
            }
            return _redisCache.StringGet(key);
        }

        public void Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException();
            }
            _redisCache.KeyDelete(key);
        }

        public bool SearchKeys(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException();
            }
            return _redisCache.KeyExists(key);
        }
    }
}

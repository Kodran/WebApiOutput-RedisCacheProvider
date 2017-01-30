namespace RedisCacheProvider
{
    public interface IRedisCacheApi
    {
        void Add(string key, string value, int expires);
        string Get(string key);
        bool SearchKeys(string key);
        void Remove(string key);
    }
}

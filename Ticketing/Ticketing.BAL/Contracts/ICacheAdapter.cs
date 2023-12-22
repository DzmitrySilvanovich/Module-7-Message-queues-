namespace Ticketing.BAL.Contracts
{
    public interface ICacheAdapter
    {
        T Get<T>(string key);

        void Set<T>(string key, T value);

        void Remove(string key);

        void Invalidate(string subKey);
    }
}

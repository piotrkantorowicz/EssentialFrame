using EssentialFrame.Cache.Base;

namespace EssentialFrame.Cache;

public class StringCache<T> : CacheBase<string, T>
{
    public StringCache(int interval = 1000) : base(interval)
    {
    }
}
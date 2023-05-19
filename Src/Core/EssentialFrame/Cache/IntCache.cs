using EssentialFrame.Cache.Base;

namespace EssentialFrame.Cache;

public class IntCache<T> : CacheBase<int, T>
{
    public IntCache(int interval = 1000) : base(interval)
    {
    }
}
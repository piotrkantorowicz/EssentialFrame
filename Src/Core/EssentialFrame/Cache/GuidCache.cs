using System;
using EssentialFrame.Cache.Base;

namespace EssentialFrame.Cache;

public class GuidCache<T> : CacheBase<Guid, T>
{
    public GuidCache(int interval = 1000) : base(interval)
    {
    }
}
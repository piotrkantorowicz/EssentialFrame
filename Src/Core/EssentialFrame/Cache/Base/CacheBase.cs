using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EssentialFrame.Cache.Interfaces;

namespace EssentialFrame.Cache.Base;

public abstract class CacheBase<TK, T> : ICache<TK, T> where TK : notnull
{
    private readonly Dictionary<TK, T> _cache = new();
    private readonly ReaderWriterLockSlim _locker = new();
    private readonly Dictionary<TK, Timer> _timers = new();
    private readonly int _timerInterval;
    private bool _disposed;

    protected CacheBase(int timerInterval)
    {
        _timerInterval = timerInterval;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Clear()
    {
        _locker.EnterWriteLock();

        try
        {
            try
            {
                foreach (Timer t in _timers.Values)
                {
                    t.Dispose();
                }
            }
            catch
            {
                // ignored
            }

            _timers.Clear();
            _cache.Clear();
        }
        finally
        {
            _locker.ExitWriteLock();
        }
    }

    public void Add(TK key, T value, int timeout, bool restartTimer = false)
    {
        if (_disposed)
        {
            return;
        }

        if (timeout != Timeout.Infinite && timeout < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout));
        }

        _locker.EnterWriteLock();

        try
        {
            CheckTimer(key, timeout, restartTimer);
            _cache[key] = value;
        }
        finally
        {
            _locker.ExitWriteLock();
        }
    }

    public void Add(TK key, T value)
    {
        Add(key, value, Timeout.Infinite);
    }

    public void Remove(Predicate<TK> pattern)
    {
        if (_disposed)
        {
            return;
        }

        _locker.EnterWriteLock();

        try
        {
            List<TK> removers = (from k in _cache.Keys where pattern(k) select k).ToList();

            foreach (TK workKey in removers)
            {
                try
                {
                    _timers[workKey].Dispose();
                }
                catch
                {
                    // ignored
                }

                _timers.Remove(workKey);
                _cache.Remove(workKey);
            }
        }
        finally
        {
            _locker.ExitWriteLock();
        }
    }

    public void Remove(TK key)
    {
        if (_disposed)
        {
            return;
        }

        _locker.EnterWriteLock();

        try
        {
            if (!_cache.ContainsKey(key))
            {
                return;
            }

            try
            {
                _timers[key].Dispose();
            }
            catch
            {
                // ignored
            }

            _timers.Remove(key);
            _cache.Remove(key);
        }
        finally
        {
            _locker.ExitWriteLock();
        }
    }

    public T this[TK key] => Get(key);

    public T Get(TK key)
    {
        if (_disposed)
        {
            return default;
        }

        _locker.EnterReadLock();

        try
        {
            return _cache.TryGetValue(key, out T rv) ? rv : default;
        }

        finally
        {
            _locker.ExitReadLock();
        }
    }

    public bool TryGet(TK key, out T value)
    {
        if (_disposed)
        {
            value = default;

            return false;
        }

        _locker.EnterReadLock();

        try
        {
            return _cache.TryGetValue(key, out value);
        }
        finally
        {
            _locker.ExitReadLock();
        }
    }

    public bool Exists(TK key)
    {
        if (_disposed)
        {
            return false;
        }

        _locker.EnterReadLock();

        try
        {
            return _cache.ContainsKey(key);
        }
        finally
        {
            _locker.ExitReadLock();
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (!disposing)
        {
            return;
        }

        Clear();
        _locker.Dispose();
    }

    private void CheckTimer(TK key, int cacheTimeout, bool restartTimerIfExists)
    {
        if (_timers.TryGetValue(key, out Timer timer))
        {
            if (restartTimerIfExists)
            {
                timer.Change(cacheTimeout == Timeout.Infinite ? Timeout.Infinite : cacheTimeout * _timerInterval,
                    Timeout.Infinite);
            }
        }
        else
        {
            _timers.Add(key,
                new Timer(RemoveByTimer!, key,
                    cacheTimeout == Timeout.Infinite ? Timeout.Infinite : cacheTimeout * _timerInterval,
                    Timeout.Infinite));
        }
    }

    private void RemoveByTimer(object state)
    {
        Remove((TK)state);
    }
}
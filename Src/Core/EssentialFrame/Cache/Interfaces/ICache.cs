using System;
using System.Collections.Generic;

namespace EssentialFrame.Cache.Interfaces;

public interface ICache<TK, T> : IDisposable
{
    T this[TK key] { get; }

    T Get(TK key);

    T Get(Func<TK, T, bool> predicate);

    IReadOnlyCollection<T> GetMany();

    IReadOnlyCollection<T> GetMany(Func<TK, T, bool> predicate);

    bool TryGet(TK key, out T value);

    bool TryGet(Func<TK, T, bool> predicate, out T value);

    bool Exists(TK key);

    bool Exists(Func<TK, T, bool> predicate);

    void Add(TK key, T value);

    void Add(TK key, T value, int timeout, bool restartTimer = false);

    void AddMany(IEnumerable<KeyValuePair<TK, T>> values);

    void AddMany(IEnumerable<KeyValuePair<TK, T>> values, int timeout, bool restartTimer = false);

    void Remove(TK key);

    void Remove(Func<TK, T, bool> predicate);

    void RemoveMany(IEnumerable<TK> keys);

    void RemoveMany(Func<TK, T, bool> predicate);

    void Clear();
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using EssentialFrame.Cache;
using EssentialFrame.Cache.Interfaces;
using FluentAssertions;
using NUnit.Framework;

namespace EssentialFrame.Tests.UnitTests.Cache;

[TestFixture]
public class CacheTests
{
    private const int TimerInterval = 1;
    private const int CacheDelay = 15;

    private readonly Faker _faker = new();
    private readonly ICache<Guid, string> _cache;

    public CacheTests()
    {
        _cache = new MemoryCache<Guid, string>(TimerInterval);
    }

    [Test]
    public void Get_Always_ShouldGetItemFromCache()
    {
        // Arrange
        Guid key = _faker.Random.Guid();
        string value = _faker.Random.String();
        _cache.Add(key, value);

        // Act
        string result = _cache.Get(key);

        // Assert
        result.Should().Be(value);
    }

    [Test]
    public void Get_AlwaysWhenUsingPredicate_ShouldGetItemFromCache()
    {
        // Arrange
        Guid key = _faker.Random.Guid();
        string value = _faker.Random.String();
        _cache.Add(key, value);

        // Act
        string result = _cache.Get((_, v) => v == value);

        // Assert
        result.Should().Be(value);
    }

    [Test]
    public void GetMany_Always_ShouldGetItemFromCache()
    {
        // Arrange
        Guid key = _faker.Random.Guid();
        string value = _faker.Random.String();
        _cache.Add(key, value);

        // Act
        IReadOnlyCollection<string> result = _cache.GetMany();

        // Assert
        result.Should().NotBeEmpty();
        result.Should().Contain(value);
    }

    [Test]
    public void GetMany_AlwaysWhenUsingPredicate_ShouldGetItemFromCache()
    {
        // Arrange
        Guid key = _faker.Random.Guid();
        string value = _faker.Random.String();
        _cache.Add(key, value);

        // Act
        IReadOnlyCollection<string> result = _cache.GetMany((_, v) => v == value);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().Contain(value);
    }

    [Test]
    public void TryGet_Always_ShouldTryGetItemFromCache()
    {
        // Arrange
        Guid key = _faker.Random.Guid();
        string value = _faker.Random.String();
        _cache.Add(key, value);

        // Act
        bool result = _cache.TryGet(key, out string resultValue);

        // Assert
        result.Should().BeTrue();
        resultValue.Should().Be(value);
    }

    [Test]
    public void TryGet_AlwaysWhenUsingPredicate_ShouldTryGetItemFromCache()
    {
        // Arrange
        Guid key = _faker.Random.Guid();
        string value = _faker.Random.String();
        _cache.Add(key, value);

        // Act
        bool result = _cache.TryGet((_, v) => v == value, out string resultValue);

        // Assert
        result.Should().BeTrue();
        resultValue.Should().Be(value);
    }

    [Test]
    public void Exists_Always_ShouldCheckItemExistsInCache()
    {
        // Arrange
        Guid key = _faker.Random.Guid();
        string value = _faker.Random.String();
        _cache.Add(key, value);

        // Act
        bool result = _cache.Exists(key);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Exists_AlwaysWhenUsingPredicate_ShouldTryGetItemFromCache()
    {
        // Arrange
        Guid key = _faker.Random.Guid();
        string value = _faker.Random.String();
        _cache.Add(key, value);

        // Act
        bool result = _cache.Exists((_, v) => v == value);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Add_Always_ShouldAddItemToCache()
    {
        // Arrange
        Guid key = _faker.Random.Guid();
        string value = _faker.Random.String();

        // Act
        _cache.Add(key, value);

        // Assert
        _cache[key].Should().Be(value);
    }

    [Test]
    public void Add_WhenItemExists_ShouldOverwriteItemInCache()
    {
        // Arrange
        Guid key = _faker.Random.Guid();
        string value = _faker.Random.String();
        _cache.Add(key, value);
        string newValue = _faker.Random.String();

        // Act
        _cache.Add(key, newValue);

        // Assert
        _cache[key].Should().Be(newValue);
    }

    [Test]
    public void Add_Always_ShouldOverwriteItemToCacheWithTimeout()
    {
        // Arrange
        Guid key = _faker.Random.Guid();
        string value = _faker.Random.String();
        int timeout = _faker.Random.Int(1, 1000);
        _cache.Add(key, value);

        // Act
        _cache.Add(key, value, timeout);

        // Assert
        _cache[key].Should().Be(value);
    }

    [Test]
    public void Add_Always_ShouldOverwriteItemToCacheWithRestartTimer()
    {
        // Arrange
        Guid key = _faker.Random.Guid();
        string value = _faker.Random.String();
        int timeout = _faker.Random.Int(1, 1000);
        _cache.Add(key, value);

        // Act
        _cache.Add(key, value, timeout, true);

        // Assert
        _cache[key].Should().Be(value);
    }

    [Test]
    public void Add_Always_ShouldAddItemToCacheWithTimeout()
    {
        // Arrange
        Guid key = _faker.Random.Guid();
        string value = _faker.Random.String();
        int timeout = _faker.Random.Int(1, 1000);

        // Act
        _cache.Add(key, value, timeout);

        // Assert
        _cache[key].Should().Be(value);
    }

    [Test]
    public void Add_Always_ShouldAddItemToCacheWithRestartTimer()
    {
        // Arrange
        Guid key = _faker.Random.Guid();
        string value = _faker.Random.String();
        int timeout = _faker.Random.Int(1, 1000);

        // Act
        _cache.Add(key, value, timeout, true);

        // Assert
        _cache[key].Should().Be(value);
    }

    [Test]
    public async Task Add_WhenTimeoutExpired_ShouldRemoveItemFromCache()
    {
        // Arrange
        Guid key = _faker.Random.Guid();
        string value = _faker.Random.String();
        int timeout = _faker.Random.Int(1, 1000);

        // Act
        _cache.Add(key, value, timeout);
        await Task.Delay(timeout + CacheDelay);

        // Assert
        _cache.Exists(key).Should().BeFalse();
    }

    [Test]
    public async Task Add_WhenTimeoutExpiredAndRestartTimer_ShouldRestartTimer()
    {
        // Arrange
        Guid key = _faker.Random.Guid();
        string value = _faker.Random.String();
        int timeout = _faker.Random.Int(1, 1000);

        // Act
        _cache.Add(key, value, timeout, true);
        await Task.Delay(timeout / 2);
        _cache.Add(key, value, timeout, true);
        await Task.Delay(timeout / 2);

        // Assert
        _cache.Exists(key).Should().BeTrue();
    }

    [Test]
    public void Add_ThenTimeoutIsZeroOrNegative_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        Guid key = _faker.Random.Guid();
        string value = _faker.Random.String();
        int timeout = _faker.Random.Int(-1000, 0);

        // Act
        Action act = () => _cache.Add(key, value, timeout);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void AddMany_Always_ShouldAddItemsToCache()
    {
        // Arrange
        Guid key1 = _faker.Random.Guid();
        string value1 = _faker.Random.String();
        Guid key2 = _faker.Random.Guid();
        string value2 = _faker.Random.String();
        Guid key3 = _faker.Random.Guid();
        string value3 = _faker.Random.String();
        int timeout = _faker.Random.Int(1, 1000);

        // Act
        _cache.AddMany(new Dictionary<Guid, string> { { key1, value1 }, { key2, value2 }, { key3, value3 } }, timeout,
            true);

        // Assert
        _cache[key1].Should().Be(value1);
        _cache[key2].Should().Be(value2);
        _cache[key3].Should().Be(value3);
    }

    [Test]
    public void AddMany_Always_AddMany_Always_ShouldAddItemsToCacheWithTimeout()
    {
        // Arrange
        Guid key1 = _faker.Random.Guid();
        string value1 = _faker.Random.String();
        Guid key2 = _faker.Random.Guid();
        string value2 = _faker.Random.String();
        Guid key3 = _faker.Random.Guid();
        string value3 = _faker.Random.String();

        // Act
        _cache.AddMany(new Dictionary<Guid, string> { { key1, value1 }, { key2, value2 }, { key3, value3 } });
    }

    [Test]
    public void Remove_Always_ShouldRemoveItemFromCache()
    {
        // Arrange
        Guid key = _faker.Random.Guid();
        string value = _faker.Random.String();
        _cache.Add(key, value);

        // Act
        _cache.Remove(key);

        // Assert
        _cache.Exists(key).Should().BeFalse();
    }

    [Test]
    public void Remove_AlwaysWhenUsingPredicate_ShouldRemoveItemFromCache()
    {
        // Arrange
        Guid key = _faker.Random.Guid();
        string value = _faker.Random.String();
        _cache.Add(key, value);

        // Act
        _cache.Remove((_, v) => v == value);

        // Assert
        _cache.Exists(key).Should().BeFalse();
    }

    [Test]
    public void RemoveMany_Always_ShouldRemoveItemsFromCache()
    {
        // Arrange
        Guid key1 = _faker.Random.Guid();
        string value1 = _faker.Random.String();
        Guid key2 = _faker.Random.Guid();
        string value2 = _faker.Random.String();
        Guid key3 = _faker.Random.Guid();
        string value3 = _faker.Random.String();
        _cache.Add(key1, value1);
        _cache.Add(key2, value2);
        _cache.Add(key3, value3);

        // Act
        _cache.RemoveMany(new List<Guid> { key1, key2 });

        // Assert
        _cache.Exists(key1).Should().BeFalse();
        _cache.Exists(key2).Should().BeFalse();
        _cache.Exists(key3).Should().BeTrue();
    }

    [Test]
    public void RemoveMany_Always_ShouldRemoveItemsFromCacheUsingPattern()
    {
        // Arrange
        Guid key1 = _faker.Random.Guid();
        string value1 = _faker.Random.String();
        Guid key2 = _faker.Random.Guid();
        string value2 = _faker.Random.String();
        Guid key3 = _faker.Random.Guid();
        string value3 = _faker.Random.String();
        _cache.Add(key1, value1);
        _cache.Add(key2, value2);
        _cache.Add(key3, value3);

        // Act
        _cache.RemoveMany((_, v) => v == value1 || v == value2);

        // Assert
        _cache.Exists(key1).Should().BeFalse();
        _cache.Exists(key2).Should().BeFalse();
        _cache.Exists(key3).Should().BeTrue();
    }

    [Test]
    public void Clear_Always_ShouldClearCache()
    {
        // Arrange
        Guid key = _faker.Random.Guid();
        string value = _faker.Random.String();
        _cache.Add(key, value);

        // Act
        _cache.Clear();

        // Assert
        _cache.Exists(key).Should().BeFalse();
    }
}
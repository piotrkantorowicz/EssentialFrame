using System;
using System.Threading.Tasks;
using Bogus;
using EssentialFrame.Cache;
using EssentialFrame.Cache.Interfaces;
using FluentAssertions;
using NUnit.Framework;

namespace EssentialFrame.UnitTests.Cache;

[TestFixture]
public class CacheTests
{
    private const int TimerInterval = 1;
    private const int CacheDelay = 10;

    private readonly Faker _faker = new();
    private readonly ICache<Guid, string> _cache;

    public CacheTests()
    {
        _cache = new GuidCache<string>(TimerInterval);
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
    public void Remove_Always_ShouldRemoveItemFromCacheByPattern()
    {
        // Arrange
        Guid key = _faker.Random.Guid();
        string value = _faker.Random.String();
        _cache.Add(key, value);

        // Act
        _cache.Remove(k => k == key);

        // Assert
        _cache.Exists(key).Should().BeFalse();
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
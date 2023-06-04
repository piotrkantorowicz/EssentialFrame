using System;
using Bogus;
using EssentialFrame.Time;
using FluentAssertions;
using NUnit.Framework;

namespace EssentialFrame.Tests.UnitTests.Time;

[TestFixture]
public class SystemClockTests
{
    private readonly Faker _faker = new();

    [Test]
    public void Set_Always_ShouldSetCustomDateTimeOffset()
    {
        // Arrange
        DateTimeOffset customDateTimeOffset = _faker.Date.FutureOffset();

        // Act
        SystemClock.Set(customDateTimeOffset);

        // Assert
        customDateTimeOffset.Should().Be(SystemClock.UtcNow);
    }

    [Test]
    public void Reset_Always_ShouldResetCustomDateTimeOffset()
    {
        // Arrange
        DateTimeOffset customDateTimeOffset = _faker.Date.FutureOffset();
        SystemClock.Set(customDateTimeOffset);

        // Act
        SystemClock.Reset();

        // Assert
        customDateTimeOffset.Should().NotBe(SystemClock.UtcNow);
    }

    [Test]
    public void Min_Always_ShouldReturnMinDateTimeOffset()
    {
        // Arrange
        DateTimeOffset minDateTimeOffset = DateTimeOffset.MinValue;

        // Act
        DateTimeOffset result = SystemClock.Min;

        // Assert
        result.Should().Be(minDateTimeOffset);
    }

    [Test]
    public void Max_Always_ShouldReturnMaxDateTimeOffset()
    {
        // Arrange
        DateTimeOffset maxDateTimeOffset = DateTimeOffset.MaxValue;

        // Act
        DateTimeOffset result = SystemClock.Max;

        // Assert
        result.Should().Be(maxDateTimeOffset);
    }
}
using Bogus;
using EssentialFrame.TestData.Domain.ValueObjects;
using FluentAssertions;
using NUnit.Framework;

namespace EssentialFrame.Domain.UnitTests.ValueObjects;

[TestFixture]
public sealed class ValueObjectTests
{
    private readonly Faker _faker = new();

    [Test]
    public void Equals_WhenObjectsHaveTheSameValues_ShouldBeTrue()
    {
        // Arrange
        string value = _faker.Lorem.Sentence();
        TestTitle title1 = new(value, true);
        TestTitle title2 = new(value, true);

        // Act
        bool isEqual = title1.Equals(title2);
        bool haveValidHashes = title1.GetHashCode() == title2.GetHashCode();
        bool isEqualsUsingOperator = title1 == title2;
        bool isNotEqualsUsingOperator = title1 != title2;

        // Assert
        isEqual.Should().BeTrue();
        haveValidHashes.Should().BeTrue();
        isEqualsUsingOperator.Should().BeTrue();
        isNotEqualsUsingOperator.Should().BeFalse();
    }

    [Test]
    public void Equals_WhenObjectsHaveDifferentValues_ShouldBeFalse()
    {
        // Arrange
        TestTitle title1 = new(_faker.Lorem.Sentence(), true);
        TestTitle title2 = new(_faker.Lorem.Sentence(), false);

        // Act
        bool isEqual = title1.Equals(title2);
        bool haveValidHashes = title1.GetHashCode() == title2.GetHashCode();
        bool isEqualsUsingOperator = title1 == title2;
        bool isNotEqualsUsingOperator = title1 != title2;

        // Assert
        isEqual.Should().BeFalse();
        haveValidHashes.Should().BeFalse();
        isEqualsUsingOperator.Should().BeFalse();
        isNotEqualsUsingOperator.Should().BeTrue();
    }
}
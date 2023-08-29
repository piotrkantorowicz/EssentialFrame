using System;
using Bogus;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;
using FluentAssertions;
using NUnit.Framework;

namespace EssentialFrame.Domain.Tests.UnitTests.ValueObjects;

[TestFixture]
public sealed class ValueObjectTests
{
    private readonly Faker _faker = new();

    [Test]
    public void Equals_WhenObjectsHaveTheSameValues_ShouldBeTrue()
    {
        // Arrange
        string value = _faker.Lorem.Sentence();
        Title title1 = Title.Default(value);
        Title title2 = Title.Default(value);

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
        Title title1 = Title.Default(_faker.Lorem.Sentence());
        Title title2 = Title.Default(_faker.Lorem.Sentence());

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

    [Test]
    public void Create_WhenTitleIsEmpty_ShouldThrowTitleValueCannotBeEmptyRuleValidationException()
    {
        // Arrange
        string value = string.Empty;

        // Act
        Action action = () => Title.Default(value);

        // Assert
        action.Should().Throw<BusinessRuleValidationException>().WithMessage(
            $"Unable to set value for ({typeof(Title).FullName}), because value cannot be empty");
    }
}
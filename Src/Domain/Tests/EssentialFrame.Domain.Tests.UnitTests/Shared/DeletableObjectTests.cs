using System;
using Bogus;
using EssentialFrame.ExampleApp.Domain.Posts.Entities.Images;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.BytesContents;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names;
using FluentAssertions;
using NUnit.Framework;

namespace EssentialFrame.Domain.Tests.UnitTests.Shared;

[TestFixture]
public class DeletableObjectTests
{
    private readonly Faker _faker = new();

    [Test]
    public void SafeDelete_Always_ShouldAssignCorrectValues()
    {
        // Arrange
        Guid entityIdentifier = _faker.Random.Guid();
        BytesContent bytes = BytesContent.Create(_faker.Random.Bytes(300));
        Name name = Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150)));
        Image image = Image.Create(entityIdentifier, name, bytes);

        // Act
        image.SafeDelete();

        // Assert
        image.DeletedDate.Should().NotBeNull();
        image.IsDeleted.Should().BeTrue();
    }

    [Test]
    public void UnDelete_Always_ShouldAssignCorrectValues()
    {
        // Arrange
        Guid entityIdentifier = _faker.Random.Guid();
        BytesContent bytes = BytesContent.Create(_faker.Random.Bytes(300));
        Name name = Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150)));
        Image image = Image.Create(entityIdentifier, name, bytes);

        // Act
        image.UnDelete();

        // Assert
        image.DeletedDate.Should().BeNull();
        image.IsDeleted.Should().BeFalse();
    }
}
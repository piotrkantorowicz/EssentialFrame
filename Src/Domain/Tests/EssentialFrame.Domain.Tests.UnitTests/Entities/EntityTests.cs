using System;
using Bogus;
using EssentialFrame.ExampleApp.Domain.Posts.Entities.Images;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.BytesContents;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names;
using FluentAssertions;
using NUnit.Framework;

namespace EssentialFrame.Domain.Tests.UnitTests.Entities;

[TestFixture]
public class EntityTests
{
    private readonly Faker _faker = new();

    [Test]
    public void CreateEntity_Always_ShouldAssignCorrectValues()
    {
        // Arrange
        Guid entityIdentifier = _faker.Random.Guid();
        BytesContent bytes = BytesContent.Create(_faker.Random.Bytes(300));
        Name name = Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150)));

        // Act
        Image image = Image.Create(entityIdentifier, name, bytes);

        // Assert
        image.EntityIdentifier.Should().Be(entityIdentifier);
        image.Name.Should().Be(name);
        image.Bytes.Should().BeEquivalentTo(bytes);
    }
}
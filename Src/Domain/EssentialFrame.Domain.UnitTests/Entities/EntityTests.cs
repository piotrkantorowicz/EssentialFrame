using System;
using Bogus;
using EssentialFrame.TestData.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace EssentialFrame.Domain.UnitTests.Entities;

[TestFixture]
public class EntityTests
{
    private readonly Faker _faker = new();

    [Test]
    public void CreateEntity_Always_ShouldAssignCorrectValues()
    {
        // Arrange
        Guid entityIdentifier = _faker.Random.Guid();
        byte[] bytes = _faker.Random.Bytes(300);
        string name = _faker.Lorem.Word();

        // Act
        TestEntity entity = new(entityIdentifier, name, bytes);

        // Assert
        entity.EntityIdentifier.Should().Be(entityIdentifier);
        entity.Name.Should().Be(name);
        entity.Bytes.Should().BeEquivalentTo(bytes);
    }
}
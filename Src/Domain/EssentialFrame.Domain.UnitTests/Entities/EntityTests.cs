using System;
using Bogus;
using EssentialFrame.Domain.Exceptions;
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
        TestEntity entity = TestEntity.Create(entityIdentifier, name, bytes);

        // Assert
        entity.EntityIdentifier.Should().Be(entityIdentifier);
        entity.Name.Should().Be(name);
        entity.Bytes.Should().BeEquivalentTo(bytes);
    }

    [Test]
    public void CreateEntity_WhenNameIsEmpty_ShouldThrowTestEntityNameCannotBeEmptyRuleValidationException()
    {
        // Arrange
        Guid entityIdentifier = _faker.Random.Guid();
        byte[] bytes = _faker.Random.Bytes(300);
        string name = string.Empty;

        // Act
        Action action = () => TestEntity.Create(entityIdentifier, name, bytes);

        // Assert
        action.Should().Throw<BusinessRuleValidationException>().WithMessage(
            $"Unable to set name for aggregate ({typeof(TestEntity).FullName}) with identifier ({entityIdentifier}), because image cannot be empty.");
    }

    [Test]
    public void CreateEntity_WhenImageBytesIsEmpty_ShouldThrowTestEntityImageCanNotBeEmptyRuleValidationException()
    {
        // Arrange
        Guid entityIdentifier = _faker.Random.Guid();
        byte[] bytes = Array.Empty<byte>();
        string name = _faker.Lorem.Word();

        // Act
        Action action = () => TestEntity.Create(entityIdentifier, name, bytes);

        // Assert
        action.Should().Throw<BusinessRuleValidationException>().WithMessage(
            $"Unable to set bytes for image ({typeof(TestEntity).FullName}) with identifier ({entityIdentifier}), because image cannot be empty.");
    }

    [Test]
    public void
        CreateEntity_WhenImageIsTooBig_ShouldThrowTestEntityBytesSizeCannotBeLargerThan2MbsRuleValidationException()
    {
        // Arrange
        Guid entityIdentifier = _faker.Random.Guid();
        byte[] bytes = _faker.Random.Bytes(1024 * 1024 * 50);
        string name = _faker.Lorem.Word();

        // Act
        Action action = () => TestEntity.Create(entityIdentifier, name, bytes);

        // Assert
        action.Should().Throw<BusinessRuleValidationException>().WithMessage(
            $"Unable to set image which has greater size than 2 Mbs into entity ({typeof(TestEntity).FullName}) with identifier ({entityIdentifier}). Provided image size ({bytes.Length})");
    }

    [Test]
    public void UpdateName_WhenProvidedNameIsEmpty_ShouldThrowTestEntityImageCanNotBeEmptyRuleValidationException()
    {
        // Arrange
        Guid entityIdentifier = _faker.Random.Guid();
        byte[] bytes = _faker.Random.Bytes(300);
        string name = _faker.Lorem.Word();

        TestEntity image = TestEntity.Create(entityIdentifier, name, bytes);

        // Act
        Action action = () => image.UpdateName(string.Empty);

        // Assert
        action.Should().Throw<BusinessRuleValidationException>().WithMessage(
            $"Unable to set name for aggregate ({typeof(TestEntity).FullName}) with identifier ({entityIdentifier}), because image cannot be empty.");
    }
}
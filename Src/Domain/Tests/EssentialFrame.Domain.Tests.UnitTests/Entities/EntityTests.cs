using System;
using Bogus;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.ExampleApp.Domain.Posts.Entities;
using EssentialFrame.Extensions;
using FluentAssertions;
using NUnit.Framework;

namespace EssentialFrame.Domain.Tests.UnitTests.Entities;

[TestFixture]
public class EntityTests
{
    private readonly Faker _faker = new();

    [Test]
    public void CreateImage_Always_ShouldAssignCorrectValues()
    {
        // Arrange
        Guid entityIdentifier = _faker.Random.Guid();
        byte[] bytes = _faker.Random.Bytes(300);
        string name = _faker.Lorem.Word();

        // Act
        Image image = Image.Create(entityIdentifier, name, bytes);

        // Assert
        image.EntityIdentifier.Should().Be(entityIdentifier);
        image.Name.Should().Be(name);
        image.Bytes.Should().BeEquivalentTo(bytes);
    }

    [Test]
    public void CreateEntity_WhenNameIsEmpty_ShouldThrowImageNameCannotBeEmptyRuleValidationException()
    {
        // Arrange
        Guid entityIdentifier = _faker.Random.Guid();
        byte[] bytes = _faker.Random.Bytes(300);
        string name = string.Empty;

        // Act
        Action action = () => Image.Create(entityIdentifier, name, bytes);

        // Assert
        action.Should().Throw<BusinessRuleValidationException>().WithMessage(
            $"Unable to set name for ({typeof(Image).FullName}) with identifier ({entityIdentifier}), because image name cannot be empty.");
    }

    [Test]
    public void CreateImage_WhenImageBytesIsEmpty_ShouldThrowImageCanNotBeEmptyRuleValidationException()
    {
        // Arrange
        Guid entityIdentifier = _faker.Random.Guid();
        byte[] bytes = Array.Empty<byte>();
        string name = _faker.Lorem.Word();

        // Act
        Action action = () => Image.Create(entityIdentifier, name, bytes);

        // Assert
        action.Should().Throw<BusinessRuleValidationException>().WithMessage(
            $"Unable to set bytes for ({typeof(Image).FullName}) with identifier ({entityIdentifier}), because image cannot be empty.");
    }

    [Test]
    public void CreateImage_WhenImageIsTooBig_ShouldThrowImageBytesSizeCannotBeLargerThan2MbsRuleValidationException()
    {
        // Arrange
        Guid entityIdentifier = _faker.Random.Guid();
        byte[] bytes = _faker.Random.Bytes(1024 * 1024 * 50);
        string name = _faker.Lorem.Word();

        // Act
        Action action = () => Image.Create(entityIdentifier, name, bytes);

        // Assert
        action.Should().Throw<BusinessRuleValidationException>().WithMessage(
            $"Unable to set image which has greater size than 2 Mbs into ({typeof(Image).FullName}) with identifier ({entityIdentifier}). Provided image size ({bytes.Length})");
    }

    [Test]
    public void UpdateImageName_WhenProvidedNameIsEmpty_ShouldThrowImageCanNotBeEmptyRuleValidationException()
    {
        // Arrange
        Guid entityIdentifier = _faker.Random.Guid();
        byte[] bytes = _faker.Random.Bytes(300);
        string name = _faker.Lorem.Word();

        Image image = Image.Create(entityIdentifier, name, bytes);

        // Act
        Action action = () => image.UpdateName(string.Empty);

        // Assert
        action.Should().Throw<BusinessRuleValidationException>().WithMessage(
            $"Unable to set name for ({image.GetTypeFullName()}) with identifier ({entityIdentifier}), because image name cannot be empty.");
    }
}
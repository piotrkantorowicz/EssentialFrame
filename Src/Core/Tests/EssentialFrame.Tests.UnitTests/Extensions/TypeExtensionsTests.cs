using EssentialFrame.Extensions;
using FluentAssertions;
using NUnit.Framework;

namespace EssentialFrame.Tests.UnitTests.Extensions;

[TestFixture]
public class TypeExtensionsTests
{
    [Test]
    public void GetTypeName_ShouldReturnTypeName()
    {
        // Arrange
        object @object = new();

        // Act
        string result = @object.GetTypeName();

        // Assert
        result.Should().Be("Object");
    }

    [Test]
    public void GetTypeFullName_ShouldReturnTypeName()
    {
        // Arrange
        object @object = new();

        // Act
        string result = @object.GetTypeFullName();

        // Assert
        result.Should().Be("System.Object");
    }

    [Test]
    public void GetClassName_ShouldReturnClassName()
    {
        // Arrange
        object @object = new();

        // Act
        string result = @object.GetClassName();

        // Assert
        result.Should().Be(@object.GetType().AssemblyQualifiedName);
    }
}
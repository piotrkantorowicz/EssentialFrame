using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bogus;
using EssentialFrame.Serialization;
using EssentialFrame.Serialization.Interfaces;
using EssentialFrame.UnitTests.Serialization.TestObjects;
using FluentAssertions;
using NUnit.Framework;

namespace EssentialFrame.UnitTests.Serialization;

[TestFixture]
public class SerializerTests
{
    private static readonly Faker Faker = new();
    private readonly ISerializer _serializer = new DefaultJsonSerializer();

    private static readonly BasicSerializationTestObject BasicObject = new()
    {
        PrimaryId = Faker.Random.Guid(),
        SecondaryId = Faker.Random.Int(),
        Name = Faker.Lorem.Sentence(),
        Number = Faker.Random.Decimal(),
        CreatedAt = Faker.Date.RecentOffset()
    };

    private static readonly SerializationTestObject ComplexObject = new(Faker.Random.Guid(), Faker.Random.Int(),
        Faker.Lorem.Sentence(), Faker.Random.Decimal(), Faker.Date.RecentOffset(),
        new List<BasicSerializationTestObject> { BasicObject });

    private static object[] _serializedTestObjects = { new object[] { BasicObject }, new object[] { ComplexObject } };

    [Test]
    [TestCaseSource(nameof(_serializedTestObjects))]
    public void Serialize_Always_ShouldSerializeObject(object serializedObject)
    {
        // Arrange
        // Act
        string result = _serializer.Serialize(serializedObject);

        // Assert
        result.Should().NotBeNullOrEmpty();
    }

    [Test]
    [TestCaseSource(nameof(_serializedTestObjects))]
    public void SerializeWithOptions_Always_ShouldSerializeObject(object serializedObject)
    {
        // Arrange
        JsonSerializerOptions options = new()
        {
            WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        // Act
        string result = _serializer.Serialize(serializedObject, options);

        // Assert
        result.Should().NotBeNullOrEmpty();
    }

    [Test]
    public void DeserializeBasicObject_Always_ShouldDeserializeObject()
    {
        // Arrange
        string serializedObj = _serializer.Serialize(BasicObject);

        // Act
        BasicSerializationTestObject result = _serializer.Deserialize<BasicSerializationTestObject>(serializedObj);

        // Assert
        result.Should().BeEquivalentTo(BasicObject);
    }

    [Test]
    public void DeserializeBasicObjectWithOptions_Always_ShouldDeserializeObject()
    {
        // Arrange
        JsonSerializerOptions options = new()
        {
            WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        string serializedObj = _serializer.Serialize(BasicObject, options);

        // Act
        SerializationTestObject result = _serializer.Deserialize<SerializationTestObject>(serializedObj, options);

        // Assert
        result.Should().BeEquivalentTo(BasicObject);
    }

    [Test]
    public void DeserializeBasicObject_Always_ShouldDeserializeObjectWithCustomType()
    {
        // Arrange
        string serializedObj = _serializer.Serialize(BasicObject);

        // Act
        BasicSerializationTestObject result =
            _serializer.Deserialize<BasicSerializationTestObject>(serializedObj, typeof(BasicSerializationTestObject));

        // Assert
        result.Should().BeEquivalentTo(BasicObject);
    }

    [Test]
    public void DeserializeBasicObjectWithOptions_Always_ShouldDeserializeObjectWithCustomType()
    {
        // Arrange
        JsonSerializerOptions options = new()
        {
            WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        string serializedObj = _serializer.Serialize(BasicObject);

        // Act
        BasicSerializationTestObject result =
            _serializer.Deserialize<BasicSerializationTestObject>(serializedObj, typeof(BasicSerializationTestObject),
                options);

        // Assert
        result.Should().BeEquivalentTo(BasicObject);
    }
    
    
    
    [Test]
    public void DeserializeBasicObject_Always_ShouldDeserializeObjectWithUnspecifiedType()
    {
        // Arrange
        string serializedObj = _serializer.Serialize(BasicObject);

        // Act
        object result = _serializer.Deserialize(serializedObj, typeof(SerializationTestObject));

        // Assert
        result.Should().BeEquivalentTo(BasicObject);
    }

    [Test]
    public void DeserializeBasicObjectWithOptions_Always_ShouldDeserializeObjectWithUnspecifiedType()
    {
        // Arrange
        JsonSerializerOptions options = new()
        {
            WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        string serializedObj = _serializer.Serialize(BasicObject);

        // Act
        object result = _serializer.Deserialize(serializedObj, typeof(SerializationTestObject), options);

        // Assert
        result.Should().BeEquivalentTo(BasicObject);
    }

    [Test]
    public void DeserializeComplexObject_Always_ShouldDeserializeObject()
    {
        // Arrange
        string serializedObj = _serializer.Serialize(ComplexObject);

        // Act
        SerializationTestObject result = _serializer.Deserialize<SerializationTestObject>(serializedObj);

        // Assert
        result.Should().BeEquivalentTo(ComplexObject);
    }

    [Test]
    public void DeserializeComplexObjectWithOptions_Always_ShouldDeserializeObject()
    {
        // Arrange
        JsonSerializerOptions options = new()
        {
            WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        string serializedObj = _serializer.Serialize(ComplexObject, options);

        // Act
        SerializationTestObject result = _serializer.Deserialize<SerializationTestObject>(serializedObj, options);

        // Assert
        result.Should().BeEquivalentTo(ComplexObject);
    }


    [Test]
    public void DeserializeComplexObject_Always_ShouldDeserializeObjectWithCustomType()
    {
        // Arrange
        string serializedObj = _serializer.Serialize(ComplexObject);

        // Act
        SerializationTestObject result =
            _serializer.Deserialize<SerializationTestObject>(serializedObj, typeof(SerializationTestObject));

        // Assert
        result.Should().BeEquivalentTo(ComplexObject);
    }

    [Test]
    public void DeserializeComplexObjectWithOptions_Always_ShouldDeserializeObjectWithCustomType()
    {
        // Arrange
        JsonSerializerOptions options = new()
        {
            WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        string serializedObj = _serializer.Serialize(ComplexObject);

        // Act
        SerializationTestObject result =
            _serializer.Deserialize<SerializationTestObject>(serializedObj, typeof(SerializationTestObject), options);

        // Assert
        result.Should().BeEquivalentTo(ComplexObject);
    }

    [Test]
    public void DeserializeComplexObject_Always_ShouldDeserializeObjectWithUnspecifiedType()
    {
        // Arrange
        string serializedObj = _serializer.Serialize(ComplexObject);

        // Act
        object result = _serializer.Deserialize(serializedObj, typeof(SerializationTestObject));

        // Assert
        result.Should().BeEquivalentTo(ComplexObject);
    }

    [Test]
    public void DeserializeComplexObjectWithOptions_Always_ShouldDeserializeObjectWithUnspecifiedType()
    {
        // Arrange
        JsonSerializerOptions options = new()
        {
            WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        string serializedObj = _serializer.Serialize(ComplexObject);

        // Act
        object result = _serializer.Deserialize(serializedObj, typeof(SerializationTestObject), options);

        // Assert
        result.Should().BeEquivalentTo(ComplexObject);
    }
}
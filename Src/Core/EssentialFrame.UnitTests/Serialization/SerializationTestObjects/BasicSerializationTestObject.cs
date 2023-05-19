using System;

namespace EssentialFrame.UnitTests.Serialization.SerializationTestObjects;

public class BasicSerializationTestObject
{
    public Guid PrimaryId { get; set; }

    public int SecondaryId { get; set; }

    public string Name { get; set; }

    public decimal Number { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
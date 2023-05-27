using System;
using System.Collections.Generic;

namespace EssentialFrame.UnitTests.Serialization.TestObjects;

public class SerializationTestObject
{
    public Guid PrimaryId { get; }

    public int SecondaryId { get; init; }

    public string Name { get; private set; }

    public decimal Number { get; protected set; }

    public DateTimeOffset CreatedAt { get; protected internal set; }

    public IEnumerable<BasicSerializationTestObject> Items { get; }

    public SerializationTestObject(Guid primaryId, int secondaryId, string name, decimal number,
        DateTimeOffset createdAt, IEnumerable<BasicSerializationTestObject> items)
    {
        PrimaryId = primaryId;
        SecondaryId = secondaryId;
        Name = name;
        Number = number;
        CreatedAt = createdAt;
        Items = items;
    }
}
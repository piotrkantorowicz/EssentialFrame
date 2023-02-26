using System;
using EssentialFrame.Domain.Entities;

namespace EssentialFrame.TestData.Domain.Entities;

public class TestEntity : Entity
{
    public TestEntity(Guid entityIdentifier, string name, byte[] bytes) : base(entityIdentifier)
    {
        Name = name;
        Bytes = bytes;
    }

    public string Name { get; private set; }

    public byte[] Bytes { get; }

    public void UpdateName(string name)
    {
        Name = name;
    }
}
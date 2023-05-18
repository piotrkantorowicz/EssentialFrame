using System;
using EssentialFrame.Domain.Entities;
using EssentialFrame.TestData.Domain.Rules;

namespace EssentialFrame.TestData.Domain.Entities;

public sealed class TestEntity : Entity
{
    private TestEntity(Guid entityIdentifier, string name, byte[] bytes) : base(entityIdentifier)
    {
        CheckRule(new TestEntityNameCannotBeEmptyRule(entityIdentifier, GetType(), name));
        CheckRule(new TestEntityImageCanNotBeEmptyRule(entityIdentifier, GetType(), bytes));
        CheckRule(new TestEntityBytesSizeCannotBeLargerThan2MbsRule(entityIdentifier, GetType(), bytes));

        Name = name;
        Bytes = bytes;
    }

    public string Name { get; private set; }

    public byte[] Bytes { get; }

    public static TestEntity Create(Guid entityIdentifier, string name, byte[] bytes)
    {
        return new TestEntity(entityIdentifier, name, bytes);
    }

    public void UpdateName(string name)
    {
        CheckRule(new TestEntityNameCannotBeEmptyRule(EntityIdentifier, GetType(), name));

        Name = name;
    }
}
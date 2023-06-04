using System;
using EssentialFrame.Domain.Entities;
using EssentialFrame.ExampleApp.Domain.Posts.Rules;

namespace EssentialFrame.ExampleApp.Domain.Posts.Entities;

public sealed class Image : Entity
{
    private Image(Guid entityIdentifier, string name, byte[] bytes) : base(entityIdentifier)
    {
        CheckRule(new ImageNameCannotBeEmptyRule(entityIdentifier, GetType(), name));
        CheckRule(new ImageCanNotBeEmptyRule(entityIdentifier, GetType(), bytes));
        CheckRule(new ImageBytesSizeCannotBeLargerThan2MbsRule(entityIdentifier, GetType(), bytes));

        Name = name;
        Bytes = bytes;
    }

    public string Name { get; private set; }

    public byte[] Bytes { get; }

    public static Image Create(Guid entityIdentifier, string name, byte[] bytes)
    {
        return new Image(entityIdentifier, name, bytes);
    }

    public void UpdateName(string name)
    {
        CheckRule(new ImageNameCannotBeEmptyRule(EntityIdentifier, GetType(), name));

        Name = name;
    }
}
using System;
using EssentialFrame.Domain.Core.Entities;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.BytesContents;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names;

namespace EssentialFrame.ExampleApp.Domain.Posts.Entities.Images;

public sealed class Image : Entity
{
    private Image(Name name, BytesContent bytes)
    {
        Name = name;
        Bytes = bytes;
    }

    private Image(Guid entityIdentifier, Name name, BytesContent bytes) : base(entityIdentifier)
    {
        Name = name;
        Bytes = bytes;
    }

    public Name Name { get; private set; }

    public BytesContent Bytes { get; }

    public static Image Create(Name name, BytesContent bytes)
    {
        return new Image(name, bytes);
    }

    public static Image Create(Guid imageIdentifier, Name name, BytesContent bytes)
    {
        return new Image(imageIdentifier, name, bytes);
    }

    public void UpdateName(Name name)
    {
        if (Name != name)
        {
            Name = name;
        }
    }
}
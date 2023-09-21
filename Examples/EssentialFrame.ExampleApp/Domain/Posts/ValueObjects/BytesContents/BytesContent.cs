using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.ExampleApp.Domain.PostComments.Shared.Rules;
using EssentialFrame.ExampleApp.Domain.Shared.Rules;

namespace EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.BytesContents;

public sealed class BytesContent : ValueObject
{
    private const int MaxMbs = 2;
    
    private BytesContent(byte[] bytes)
    {
        CheckRule(new BytesCannotBeEmptyRule(GetType(), bytes));
        CheckRule(new BytesContentCannotBeLargerThanSpecifiedMbsRule(GetType(), bytes, MaxMbs));

        Bytes = bytes;
    }

    public byte[] Bytes { get; }

    public int Length => Bytes.Length;

    public static BytesContent Create(byte[] bytes)
    {
        return new BytesContent(bytes);
    }

    public static BytesContent Empty()
    {
        return new BytesContent(Array.Empty<byte>());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Bytes;
    }
}
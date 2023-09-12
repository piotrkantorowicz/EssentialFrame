using System;
using System.Collections.Generic;
using EssentialFrame.Domain.ValueObjects.Core;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.BytesContents.Rules;

namespace EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.BytesContents;

public sealed class BytesContent : ValueObject
{
    private BytesContent(byte[] bytes)
    {
        CheckRule(new BytesContentCannotBeEmpty(GetType(), bytes));
        CheckRule(new BytesContentCannotBeLargerThan2MbsRule(GetType(), bytes));

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
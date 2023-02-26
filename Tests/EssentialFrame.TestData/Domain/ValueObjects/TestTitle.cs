using System;
using System.Collections.Generic;
using EssentialFrame.Domain.ValueObjects;

namespace EssentialFrame.TestData.Domain.ValueObjects;

public class TestTitle : ValueObject
{
    public TestTitle(string value, bool uppercase)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("Title value cannot be null or empty.");
        }

        Value = value;
        Uppercase = uppercase;
    }

    public string Value { get; }

    public bool Uppercase { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return Uppercase;
    }
}
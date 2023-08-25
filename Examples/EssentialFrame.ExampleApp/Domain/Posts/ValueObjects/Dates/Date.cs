using System;
using System.Collections.Generic;
using EssentialFrame.Domain.ValueObjects;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates.Rules;

namespace EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;

public sealed class Date : ValueObject
{
    private Date(DateTimeOffset value)
    {
        CheckRule(new DateCanNotBeNullOrEmptyRule(GetType(), value));
        CheckRule(new DateMustBeInMinAndMaxSystemDateTimeRule(GetType(), value));

        Value = value;
    }

    public DateTimeOffset Value { get; }

    public static Date Create(DateTimeOffset value)
    {
        return new Date(value);
    }

    public bool GreaterThan(DateTimeOffset dateTimeOffset)
    {
        return Value > dateTimeOffset;
    }

    public bool LessThan(DateTimeOffset dateTimeOffset)
    {
        return Value < dateTimeOffset;
    }

    public bool GreaterThanOrEqual(DateTimeOffset dateTimeOffset)
    {
        return Value >= dateTimeOffset;
    }

    public bool LessThanOrEqual(DateTimeOffset dateTimeOffset)
    {
        return Value <= dateTimeOffset;
    }

    public bool Between(DateTimeOffset start, DateTimeOffset end)
    {
        return Value >= start && Value <= end;
    }

    public bool InRange(DateTimeOffset start, DateTimeOffset end)
    {
        return Value >= start && Value <= end;
    }

    public bool OutOfRange(DateTimeOffset start, DateTimeOffset end)
    {
        return Value < start || Value > end;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
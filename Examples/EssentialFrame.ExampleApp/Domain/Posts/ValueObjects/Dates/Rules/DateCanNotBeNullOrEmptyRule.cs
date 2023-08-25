using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Rules.Base;

namespace EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates.Rules;

public class DateCanNotBeNullOrEmptyRule : BusinessRule
{
    private readonly DateTimeOffset _value;

    public DateCanNotBeNullOrEmptyRule(Type type, DateTimeOffset value) : base(type,
        BusinessRuleTypes.ValueObjectBusinessRule)
    {
        _value = value;
    }

    public override bool IsBroken()
    {
        return _value == DateTimeOffset.MinValue || _value == DateTimeOffset.MaxValue;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("Date", _value);
    }

    public override string Message => $"({DomainObjectType.FullName}) can not be null or empty";
}
using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Core.Rules.Base;

namespace EssentialFrame.ExampleApp.Domain.PostComments.Shared.Rules;

internal sealed class DateMustBeInMinAndMaxSystemDateTimeRule : BusinessRule
{
    private readonly DateTimeOffset _value;

    public DateMustBeInMinAndMaxSystemDateTimeRule(Type type, DateTimeOffset value) : base(type,
        BusinessRuleTypes.ValueObjectBusinessRule)
    {
        _value = value;
    }

    public override bool IsBroken()
    {
        return _value < DateTimeOffset.MinValue || _value > DateTimeOffset.MaxValue;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("Date", _value);
    }

    public override string Message => $"({DomainObjectType.FullName}) must be in min and max system date time";
}
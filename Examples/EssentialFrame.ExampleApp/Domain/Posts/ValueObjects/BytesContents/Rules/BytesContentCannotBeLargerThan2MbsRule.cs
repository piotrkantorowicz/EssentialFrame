using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Core.Rules.Base;

namespace EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.BytesContents.Rules;

public sealed class BytesContentCannotBeLargerThan2MbsRule : BusinessRule
{
    private readonly byte[] _bytes;

    public BytesContentCannotBeLargerThan2MbsRule(Type domainObjectType, byte[] bytes) : base(domainObjectType,
        BusinessRuleTypes.ValueObjectBusinessRule)
    {
        _bytes = bytes;
    }

    public override string Message =>
        $"Unable to set value for ({DomainObjectType.FullName}), because value cannot be larger than 2 Mbs";

    public override bool IsBroken()
    {
        return _bytes?.Length > 2 * 1024 * 1024;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("Bytes", _bytes);
    }
}
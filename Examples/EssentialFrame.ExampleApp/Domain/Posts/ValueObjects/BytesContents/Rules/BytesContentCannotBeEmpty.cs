using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Core.Rules.Base;

namespace EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.BytesContents.Rules;

public class BytesContentCannotBeEmpty : BusinessRule
{
    private readonly byte[] _bytes;

    public BytesContentCannotBeEmpty(Type domainObjectType, byte[] bytes) : base(domainObjectType,
        BusinessRuleTypes.ValueObjectBusinessRule)
    {
        _bytes = bytes;
    }

    public override string Message =>
        $"Unable to set value for ({DomainObjectType.FullName}), because value cannot be empty";

    public override bool IsBroken()
    {
        return _bytes?.Length == 0;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("Bytes", _bytes);
    }
}
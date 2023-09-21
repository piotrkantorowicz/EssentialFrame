using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Core.Rules.Base;

namespace EssentialFrame.ExampleApp.Domain.PostComments.Shared.Rules;

internal sealed class BytesContentCannotBeLargerThanSpecifiedMbsRule : BusinessRule
{
    private readonly byte[] _bytes;
    private readonly int _mbs;

    public BytesContentCannotBeLargerThanSpecifiedMbsRule(Type domainObjectType, byte[] bytes, int mbs) : base(
        domainObjectType,
        BusinessRuleTypes.ValueObjectBusinessRule)
    {
        _bytes = bytes;
        _mbs = mbs;
    }

    public override string Message =>
        $"Unable to set value for ({DomainObjectType.FullName}), because value cannot be larger than 2 Mbs";

    public override bool IsBroken()
    {
        return _bytes?.Length > _mbs * 1024 * 1024;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("Bytes", _bytes);
    }
}
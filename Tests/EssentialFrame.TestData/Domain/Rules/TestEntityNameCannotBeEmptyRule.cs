using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Rules.Base;

namespace EssentialFrame.TestData.Domain.Rules;

public sealed class TestEntityNameCannotBeEmptyRule : EntityBusinessRuleBase
{
    private readonly string _imageName;

    public TestEntityNameCannotBeEmptyRule(Guid entityIdentifier, Type entityType, string imageName) : base(
        entityIdentifier, entityType)
    {
        _imageName = imageName;
    }

    public override string Message =>
        $"Unable to set name for aggregate ({EntityType.FullName}) with identifier ({EntityIdentifier}), because image cannot be empty.";

    public override bool IsBroken()
    {
        return string.IsNullOrEmpty(_imageName);
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("ImageName", _imageName);
    }
}
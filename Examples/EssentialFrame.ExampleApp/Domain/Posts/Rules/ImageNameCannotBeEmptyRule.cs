using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Rules.Base;

namespace EssentialFrame.ExampleApp.Domain.Posts.Rules;

public sealed class ImageNameCannotBeEmptyRule : EntityBusinessRuleBase
{
    private readonly string _imageName;

    public ImageNameCannotBeEmptyRule(Guid entityIdentifier, Type entityType, string imageName) : base(entityIdentifier,
        entityType)
    {
        _imageName = imageName;
    }

    public override string Message =>
        $"Unable to set name for ({EntityType.FullName}) with identifier ({EntityIdentifier}), because image name cannot be empty.";

    public override bool IsBroken()
    {
        return string.IsNullOrEmpty(_imageName);
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("ImageName", _imageName);
    }
}
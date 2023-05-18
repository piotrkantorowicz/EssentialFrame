using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Rules.Base;

namespace EssentialFrame.TestData.Domain.Rules;

public class TestEntityImageCanNotBeEmptyRule : EntityBusinessRuleBase
{
    private readonly byte[] _imageBytes;

    public TestEntityImageCanNotBeEmptyRule(Guid entityIdentifier, Type entityType, byte[] imageBytes) : base(
        entityIdentifier, entityType)
    {
        _imageBytes = imageBytes;
    }

    public override string Message =>
        $"Unable to set bytes for image ({EntityType.FullName}) with identifier ({EntityIdentifier}), because image cannot be empty.";

    public override bool IsBroken()
    {
        return _imageBytes is null || _imageBytes.Length == 0;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("ImageSize", _imageBytes.Length);
    }
}
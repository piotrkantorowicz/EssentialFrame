using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Rules.Base;

namespace EssentialFrame.TestData.Domain.Rules;

public sealed class TestEntityBytesSizeCannotBeLargerThan2MbsRule : EntityBusinessRuleBase
{
    private readonly byte[] _imageBytes;

    public TestEntityBytesSizeCannotBeLargerThan2MbsRule(Guid entityIdentifier, Type entityType, byte[] imageBytes) :
        base(entityIdentifier, entityType)
    {
        _imageBytes = imageBytes;
    }

    public override string Message =>
        $"Unable to set image which has greater size than 2 Mbs into entity ({EntityType.FullName}) with identifier ({EntityIdentifier}). Provided image size ({_imageBytes.Length})";

    public override bool IsBroken()
    {
        return _imageBytes.Length > 1024 * 1024 * 2;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("ImageSize", _imageBytes.Length);
    }
}
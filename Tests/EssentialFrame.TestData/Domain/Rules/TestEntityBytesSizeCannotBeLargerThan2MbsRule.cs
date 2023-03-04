using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Rules;

namespace EssentialFrame.TestData.Domain.Rules;

public sealed class TestEntityBytesSizeCannotBeLargerThan2MbsRule : BaseBusinessRule
{
    private readonly byte[] _imageBytes;

    public TestEntityBytesSizeCannotBeLargerThan2MbsRule(Guid aggregateIdentifier, Type aggregateType,
        byte[] imageBytes) : base(aggregateIdentifier, aggregateType)
    {
        _imageBytes = imageBytes ?? throw new ArgumentNullException(nameof(imageBytes));
    }

    public override string Message =>
        $"Unable to set image with has greater size than 2 Mbs into aggregate ({_aggregateType.FullName}) with identifier ({_aggregateIdentifier}). Provided image size ({_imageBytes.Length})";

    public override bool IsBroken()
    {
        return _imageBytes.Length <= 2048;
    }

    protected override void AddExtraParameters()
    {
        Parameters.TryAdd("ImageSize", _imageBytes.Length);
    }
}
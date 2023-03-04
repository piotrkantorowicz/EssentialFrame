using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Rules;

namespace EssentialFrame.TestData.Domain.Rules;

public class TestEntityImageCanNotBeEmptyRule : BaseBusinessRule
{
    private readonly byte[] _imageBytes;

    public TestEntityImageCanNotBeEmptyRule(Guid aggregateIdentifier, Type aggregateType, byte[] imageBytes) : base(
        aggregateIdentifier, aggregateType)
    {
        _imageBytes = imageBytes ?? throw new ArgumentNullException(nameof(imageBytes));
    }

    public override string Message =>
        $"Unable to set image into aggregate ({_aggregateType.FullName}) with identifier ({_aggregateIdentifier}), because image cannot be empty.";

    public override bool IsBroken()
    {
        return _imageBytes.Length == 0;
    }

    protected override void AddExtraParameters()
    {
        Parameters.TryAdd("ImageSize", _imageBytes.Length);
    }
}
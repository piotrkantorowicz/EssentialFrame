using System;
using System.Collections.Generic;
using System.Linq;
using EssentialFrame.Domain.Rules.Base;

namespace EssentialFrame.ExampleApp.Domain.Posts.Rules;

public class ImageNameMustBeUniqueWithInPostRule : AggregateBusinessRuleBase
{
    private readonly string[] _aggregateExistingImagesNames;
    private readonly string _imageName;

    public ImageNameMustBeUniqueWithInPostRule(Guid aggregateIdentifier, Type aggregateType, string imageName,
        string[] aggregateExistingImagesNames) : base(aggregateIdentifier, aggregateType)
    {
        _imageName = imageName;
        _aggregateExistingImagesNames = aggregateExistingImagesNames;
    }

    public override string Message =>
        $"Image with name {_imageName} has been already added into ({AggregateType.FullName}) with" +
        $" identifier ({AggregateIdentifier})";


    public override bool IsBroken()
    {
        if (_aggregateExistingImagesNames is null || !_aggregateExistingImagesNames.Any())
        {
            return false;
        }

        return _aggregateExistingImagesNames.Contains(_imageName);
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("ImageName", _imageName);
        Parameters.TryAdd("ExistingImageNames", _aggregateExistingImagesNames);
    }
}
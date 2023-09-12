using System;
using System.Collections.Generic;
using System.Linq;
using EssentialFrame.Domain.Rules.Base;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names;

namespace EssentialFrame.ExampleApp.Domain.Posts.Aggregates.Rules;

public class PostMustHaveOnlyUniqueImagesRule : IdentifiableBusinessRule<PostIdentifier>
{
    private readonly Name[] _aggregateExistingImagesNames;
    private readonly Name _imageName;

    public PostMustHaveOnlyUniqueImagesRule(PostIdentifier domainObjectIdentifier, Type businessObjectType,
        Name imageName,
        Name[] aggregateExistingImagesNames) : base(domainObjectIdentifier, businessObjectType,
        BusinessRuleTypes.AggregateBusinessRule)
    {
        _imageName = imageName;
        _aggregateExistingImagesNames = aggregateExistingImagesNames;
    }

    public override string Message =>
        $"Image with name {_imageName.Value} has been already added into ({DomainObjectType.FullName}) with identifier ({DomainObjectIdentifier})";


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
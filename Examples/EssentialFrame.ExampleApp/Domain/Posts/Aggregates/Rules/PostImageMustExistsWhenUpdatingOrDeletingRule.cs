using System;
using EssentialFrame.Domain.Rules.Base;
using EssentialFrame.ExampleApp.Domain.Posts.Entities.Images;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Domain.Posts.Aggregates.Rules;

public class PostImageMustExistsWhenUpdatingOrDeletingRule : IdentifiableBusinessRule<PostIdentifier>
{
    private readonly Image _image;

    public PostImageMustExistsWhenUpdatingOrDeletingRule(PostIdentifier domainObjectIdentifier, Type businessObjectType,
        Image image) : base(domainObjectIdentifier, businessObjectType, BusinessRuleTypes.AggregateBusinessRule)
    {
        _image = image;
    }

    public override string Message =>
        $"{typeof(Image).FullName} must exists inside ({DomainObjectType.FullName}) with identifier ({DomainObjectIdentifier}) to be able for update or delete";

    public override bool IsBroken()
    {
        return _image is null;
    }

    public override void AddExtraParameters()
    {
    }
}
using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Rules.Base;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Domain.PostComments.Aggregates.Rules;

public class PostCommentCanBeEditedOnlyByAuthorRule : BusinessRule
{
    private readonly UserIdentifier _authorIdentifier;
    private readonly UserIdentifier _updaterIdentifier;

    public PostCommentCanBeEditedOnlyByAuthorRule(Guid domainObjectIdentifier, Type businessObjectType,
        UserIdentifier authorIdentifier, UserIdentifier updaterIdentifier) : base(domainObjectIdentifier,
        businessObjectType, BusinessRuleTypes.AggregateBusinessRule)
    {
        _authorIdentifier = authorIdentifier;
        _updaterIdentifier = updaterIdentifier;
    }

    public override string Message =>
        $"This ({DomainObjectType.FullName}) with identifier ({DomainObjectIdentifier}) can be updated only by author. Creator identifier ({_authorIdentifier}), updater identifier ({_updaterIdentifier})";

    public override bool IsBroken()
    {
        return _authorIdentifier != _updaterIdentifier;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("CreatorIdentifier", _authorIdentifier);
        Parameters.TryAdd("UpdaterIdentifier", _updaterIdentifier);
    }
}
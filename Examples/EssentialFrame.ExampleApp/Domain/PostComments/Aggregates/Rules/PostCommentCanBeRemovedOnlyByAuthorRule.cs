using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Rules.Base;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Domain.PostComments.Aggregates.Rules;

public class PostCommentCanBeRemovedOnlyByAuthorRule : BusinessRule
{
    private readonly UserIdentifier _authorIdentifier;
    private readonly UserIdentifier _removerIdentifier;

    public PostCommentCanBeRemovedOnlyByAuthorRule(Guid domainObjectIdentifier, Type businessObjectType,
        UserIdentifier authorIdentifier, UserIdentifier removerIdentifier) : base(domainObjectIdentifier,
        businessObjectType, BusinessRuleTypes.AggregateBusinessRule)
    {
        _authorIdentifier = authorIdentifier;
        _removerIdentifier = removerIdentifier;
    }

    public override string Message =>
        $"This ({DomainObjectType.FullName}) with identifier ({DomainObjectIdentifier}) can be removed only by author. Creator identifier ({_authorIdentifier}), remover identifier ({_removerIdentifier})";

    public override bool IsBroken()
    {
        return _authorIdentifier != _removerIdentifier;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("CreatorIdentifier", _authorIdentifier);
        Parameters.TryAdd("UpdaterIdentifier", _removerIdentifier);
    }
}
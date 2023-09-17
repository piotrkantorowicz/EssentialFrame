using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Core.Rules.Base;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Domain.PostComments.Aggregates.Rules;

public class PostCommentCanBeRemovedOnlyByAuthorRule : IdentifiableBusinessRule<PostCommentIdentifier>
{
    private readonly AuthorIdentifier _authorIdentifier;
    private readonly AuthorIdentifier _removerIdentifier;

    public PostCommentCanBeRemovedOnlyByAuthorRule(PostCommentIdentifier domainObjectIdentifier,
        Type businessObjectType, AuthorIdentifier authorIdentifier, AuthorIdentifier removerIdentifier) : base(
        domainObjectIdentifier,
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
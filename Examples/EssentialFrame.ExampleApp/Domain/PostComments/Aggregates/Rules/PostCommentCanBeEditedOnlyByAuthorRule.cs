using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Rules.Base;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Domain.PostComments.Aggregates.Rules;

public class PostCommentCanBeEditedOnlyByAuthorRule : BusinessRule
{
    private readonly UserIdentifier _authorIdentifier;
    private readonly UserIdentifier _editorIdentifier;

    public PostCommentCanBeEditedOnlyByAuthorRule(Guid domainObjectIdentifier, Type businessObjectType,
        UserIdentifier authorIdentifier, UserIdentifier editorIdentifier) : base(domainObjectIdentifier,
        businessObjectType, BusinessRuleTypes.AggregateBusinessRule)
    {
        _authorIdentifier = authorIdentifier;
        _editorIdentifier = editorIdentifier;
    }

    public override string Message =>
        $"This ({DomainObjectType.FullName}) with identifier ({DomainObjectIdentifier}) can be updated only by author. Creator identifier ({_authorIdentifier}), updater identifier ({_editorIdentifier})";

    public override bool IsBroken()
    {
        return _authorIdentifier != _editorIdentifier;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("CreatorIdentifier", _authorIdentifier);
        Parameters.TryAdd("UpdaterIdentifier", _editorIdentifier);
    }
}
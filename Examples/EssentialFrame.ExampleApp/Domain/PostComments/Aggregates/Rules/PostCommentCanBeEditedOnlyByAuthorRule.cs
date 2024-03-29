﻿using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Core.Rules.Base;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Domain.PostComments.Aggregates.Rules;

public class PostCommentCanBeEditedOnlyByAuthorRule : IdentifiableBusinessRule<PostCommentIdentifier, Guid>
{
    private readonly AuthorIdentifier _authorIdentifier;
    private readonly AuthorIdentifier _editorIdentifier;

    public PostCommentCanBeEditedOnlyByAuthorRule(PostCommentIdentifier domainObjectIdentifier, Type businessObjectType,
        AuthorIdentifier authorIdentifier, AuthorIdentifier editorIdentifier) : base(domainObjectIdentifier,
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
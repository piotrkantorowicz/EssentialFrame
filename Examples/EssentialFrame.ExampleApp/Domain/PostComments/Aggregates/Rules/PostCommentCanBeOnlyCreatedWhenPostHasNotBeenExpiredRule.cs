﻿using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.Rules.Base;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.Extensions;

namespace EssentialFrame.ExampleApp.Domain.PostComments.Aggregates.Rules;

public class PostCommentCanBeOnlyCreatedWhenPostHasNotBeenExpiredRule : BusinessRule
{
    private readonly PostIdentifier _postIdentifier;
    private readonly IAggregateRepository _aggregateRepository;

    public PostCommentCanBeOnlyCreatedWhenPostHasNotBeenExpiredRule(Guid domainObjectIdentifier,
        Type businessObjectType, PostIdentifier postIdentifier, IAggregateRepository aggregateRepository) : base(
        domainObjectIdentifier, businessObjectType, BusinessRuleTypes.AggregateBusinessRule)
    {
        _postIdentifier = postIdentifier;
        _aggregateRepository = aggregateRepository;
    }

    public override string Message =>
        $"This ({DomainObjectType.FullName}) with identifier ({DomainObjectIdentifier}) can be created only when post has not been expired. Post identifier ({_postIdentifier})";

    public override bool IsBroken()
    {
        Post post = _aggregateRepository.Get<Post>(_postIdentifier.Identifier);

        if (post.State is PostState postState)
        {
            return postState.IsExpired;
        }

        throw new InvalidCastException(
            $"Unable to cast {post.State.GetTypeFullName()} to {typeof(PostState).FullName}");
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("PostIdentifier", _postIdentifier);
    }
}
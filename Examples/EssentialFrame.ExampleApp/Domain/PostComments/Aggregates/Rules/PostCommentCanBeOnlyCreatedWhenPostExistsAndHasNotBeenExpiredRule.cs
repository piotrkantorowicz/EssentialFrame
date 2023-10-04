using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Core.Rules.Base;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.Repositories;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.Extensions;

namespace EssentialFrame.ExampleApp.Domain.PostComments.Aggregates.Rules;

public class
    PostCommentCanBeOnlyCreatedWhenPostExistsAndHasNotBeenExpiredRule : IdentifiableBusinessRule<PostCommentIdentifier,
        Guid>
{
    private readonly PostIdentifier _postIdentifier;
    private readonly IPostRepository _aggregateRepository;

    public PostCommentCanBeOnlyCreatedWhenPostExistsAndHasNotBeenExpiredRule(
        PostCommentIdentifier domainObjectIdentifier, Type businessObjectType, PostIdentifier postIdentifier,
        IPostRepository aggregateRepository) : base(domainObjectIdentifier,
        businessObjectType, BusinessRuleTypes.AggregateBusinessRule)
    {
        _postIdentifier = postIdentifier;
        _aggregateRepository = aggregateRepository;
    }

    public override string Message =>
        $"This ({DomainObjectType.FullName}) with identifier ({DomainObjectIdentifier}) can be created only when post exists and has not been expired. Post identifier ({_postIdentifier})";

    public override bool IsBroken()
    {
        Post post = _aggregateRepository.Get(_postIdentifier);

        if (post is null)
        {
            return true;
        }

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
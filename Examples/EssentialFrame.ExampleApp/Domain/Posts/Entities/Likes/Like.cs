using System;
using EssentialFrame.Domain.Entities;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Votes;

namespace EssentialFrame.ExampleApp.Domain.Posts.Entities.Likes;

public class Like : Entity
{
    private Like(Guid likeIdentifier, Vote vote, Guid userId) : base(likeIdentifier)
    {
        Vote = vote;
        UserId = userId;
    }

    public Vote Vote { get; }

    public Guid UserId { get; }

    public static Like UpVote(Guid likeIdentifier, Guid userId)
    {
        return new Like(likeIdentifier, Vote.UpVote, userId);
    }

    public static Like DownVote(Guid likeIdentifier, Guid userId)
    {
        return new Like(likeIdentifier, Vote.DownVote, userId);
    }
}
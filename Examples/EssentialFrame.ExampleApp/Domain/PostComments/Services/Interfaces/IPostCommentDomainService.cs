using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Domain.Core.Events;
using EssentialFrame.ExampleApp.Domain.PostComments.Aggregates;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.CommentTexts;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.DeletedReasons;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Domain.PostComments.Services.Interfaces;

public interface IPostCommentDomainService
{
    PostComment CreateNew(PostCommentIdentifier postCommentIdentifier, PostIdentifier postIdentifier,
        PostCommentText text, DomainIdentity domainIdentity);

    Task<PostComment> CreateNewAsync(PostCommentIdentifier replyToPostCommentIdentifier, PostIdentifier postIdentifier,
        PostCommentText text, DomainIdentity domainIdentity, CancellationToken cancellationToken = default);

    PostComment InReplyTo(PostCommentIdentifier replyToPostCommentIdentifier, PostCommentText text,
        DomainIdentity domainIdentity);

    Task<PostComment> InReplyToAsync(PostCommentIdentifier replyToIdentifier, PostCommentText text,
        DomainIdentity domainIdentity, CancellationToken cancellationToken = default);

    PostComment Edit(PostCommentIdentifier postCommentIdentifier, PostCommentText text, DomainIdentity domainIdentity);

    Task<PostComment> EditAsync(PostCommentIdentifier postCommentIdentifier, PostCommentText text,
        DomainIdentity domainIdentity, CancellationToken cancellationToken = default);

    PostComment Remove(PostCommentIdentifier postCommentIdentifier, DeletedReason reason,
        DomainIdentity domainIdentity);

    Task<PostComment> RemoveAsync(PostCommentIdentifier postCommentIdentifier, DeletedReason reason,
        DomainIdentity domainIdentity, CancellationToken cancellationToken = default);
}
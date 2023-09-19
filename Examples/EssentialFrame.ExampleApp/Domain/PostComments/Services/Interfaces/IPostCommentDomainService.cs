using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.ExampleApp.Domain.PostComments.Aggregates;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.CommentTexts;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.DeletedReasons;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.PostComments.Services.Interfaces;

public interface IPostCommentDomainService
{
    PostComment CreateNew(PostCommentIdentifier postCommentIdentifier, PostIdentifier postIdentifier,
        PostCommentText text, IIdentityContext identityContext);

    Task<PostComment> CreateNewAsync(PostCommentIdentifier replyToPostCommentIdentifier, PostIdentifier postIdentifier,
        PostCommentText text, IIdentityContext identityContext, CancellationToken cancellationToken = default);

    PostComment InReplyTo(PostCommentIdentifier replyToPostCommentIdentifier, PostCommentText text,
        IIdentityContext identityContext);

    Task<PostComment> InReplyToAsync(PostCommentIdentifier replyToIdentifier, PostCommentText text,
        IIdentityContext identityContext, CancellationToken cancellationToken = default);

    PostComment Edit(PostCommentIdentifier postCommentIdentifier, PostCommentText text,
        IIdentityContext identityContext);

    Task<PostComment> EditAsync(PostCommentIdentifier postCommentIdentifier, PostCommentText text,
        IIdentityContext identityContext, CancellationToken cancellationToken = default);

    PostComment Remove(PostCommentIdentifier postCommentIdentifier, DeletedReason reason,
        IIdentityContext identityContext);

    Task<PostComment> RemoveAsync(PostCommentIdentifier postCommentIdentifier, DeletedReason reason,
        IIdentityContext identityContext, CancellationToken cancellationToken = default);
}
using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Persistence.Services.Interfaces;
using EssentialFrame.ExampleApp.Domain.PostComments.Aggregates;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.DeletedReasons;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.DomainEvents.Events;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents.Handlers;

internal sealed class PostDeletedDomainEventHandler : IEventHandler<PostDeletedDomainEvent, PostIdentifier>,
    IAsyncEventHandler<PostDeletedDomainEvent, PostIdentifier>
{
    private readonly IAggregateRepository<PostComment, PostCommentIdentifier> _postCommentRepository;

    public PostDeletedDomainEventHandler(IAggregateRepository<PostComment, PostCommentIdentifier> postCommentRepository)
    {
        _postCommentRepository =
            postCommentRepository ?? throw new ArgumentNullException(nameof(postCommentRepository));
    }

    public void Handle(PostDeletedDomainEvent @event)
    {
        foreach (PostCommentIdentifier postCommentIdentifier in @event.PostCommentIdentifiers)
        {
            PostComment postComment = _postCommentRepository.Get(postCommentIdentifier);

            postComment.Remove(DeletedReason.PostDeleted(PredefinedDeletedReasons.PostDeleted),
                @event.DomainEventIdentity);

            _postCommentRepository.Save(postComment);
        }
    }

    public async Task HandleAsync(PostDeletedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        foreach (PostCommentIdentifier postCommentIdentifier in @event.PostCommentIdentifiers)
        {
            PostComment postComment = await _postCommentRepository.GetAsync(postCommentIdentifier, cancellationToken);

            postComment.Remove(DeletedReason.PostDeleted(PredefinedDeletedReasons.PostDeleted),
                @event.DomainEventIdentity);

            await _postCommentRepository.SaveAsync(postComment, cancellationToken);
        }
    }
}
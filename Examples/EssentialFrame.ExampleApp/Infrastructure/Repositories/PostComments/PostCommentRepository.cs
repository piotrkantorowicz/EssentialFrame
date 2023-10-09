using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Domain.Persistence.Services.Interfaces;
using EssentialFrame.ExampleApp.Domain.PostComments.Aggregates;
using EssentialFrame.ExampleApp.Domain.PostComments.Repositories;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Infrastructure.Repositories.PostComments;

internal sealed class PostCommentRepository : IPostCommentRepository
{
    private readonly IAggregateRepository<PostComment, PostCommentIdentifier, Guid> _aggregateRepository;

    public PostCommentRepository(IAggregateRepository<PostComment, PostCommentIdentifier, Guid> aggregateRepository)
    {
        _aggregateRepository = aggregateRepository ?? throw new ArgumentNullException(nameof(aggregateRepository));
    }

    public PostComment Get(PostCommentIdentifier postCommentIdentifier)
    {
        return _aggregateRepository.Get(postCommentIdentifier);
    }

    public async Task<PostComment> GetAsync(PostCommentIdentifier postCommentIdentifier,
        CancellationToken cancellationToken)
    {
        return await _aggregateRepository.GetAsync(postCommentIdentifier, cancellationToken);
    }

    public void Save(PostComment postComment)
    {
        _aggregateRepository.Save(postComment);
    }

    public async Task SaveAsync(PostComment postComment, CancellationToken cancellationToken)
    {
        await _aggregateRepository.SaveAsync(postComment, cancellationToken);
    }
}
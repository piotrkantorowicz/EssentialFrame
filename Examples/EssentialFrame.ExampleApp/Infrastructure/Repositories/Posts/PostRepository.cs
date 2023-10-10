using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.Repositories;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Infrastructure.Repositories.Posts;

internal sealed class PostRepository : IPostRepository
{
    private readonly IEventSourcingAggregateRepository<Post, PostIdentifier, Guid> _aggregateRepository;

    public PostRepository(IEventSourcingAggregateRepository<Post, PostIdentifier, Guid> aggregateRepository)
    {
        _aggregateRepository = aggregateRepository ?? throw new ArgumentNullException(nameof(aggregateRepository));
    }


    public Post Get(PostIdentifier postIdentifier)
    {
        return _aggregateRepository.Get(postIdentifier);
    }

    public async Task<Post> GetAsync(PostIdentifier postIdentifier, CancellationToken cancellationToken)
    {
        return await _aggregateRepository.GetAsync(postIdentifier, cancellationToken);
    }

    public void Save(Post post)
    {
        _aggregateRepository.Save(post);
    }

    public async Task SaveAsync(Post post, CancellationToken cancellationToken)
    {
        await _aggregateRepository.SaveAsync(post, cancellationToken: cancellationToken);
    }

    public Post Unbox(PostIdentifier postIdentifier)
    {
        return _aggregateRepository.Unbox(postIdentifier);
    }

    public async Task<Post> UnboxAsync(PostIdentifier postIdentifier, CancellationToken cancellationToken)
    {
        return await _aggregateRepository.UnboxAsync(postIdentifier, cancellationToken);
    }

    public void Box(PostIdentifier postIdentifier)
    {
        _aggregateRepository.Box(postIdentifier);
    }

    public async Task BoxAsync(PostIdentifier postIdentifier, CancellationToken cancellationToken)
    {
        await _aggregateRepository.BoxAsync(postIdentifier, cancellationToken);
    }
}
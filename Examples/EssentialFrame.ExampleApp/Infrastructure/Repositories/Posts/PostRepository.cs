using EssentialFrame.Domain.Core.Events.Services.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.Persistence.Mappers.Interfaces;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.Repositories;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Infrastructure.Repositories.Posts;

internal sealed class PostRepository : EventSourcingAggregateRepository<Post, PostIdentifier>, IPostRepository
{
    public PostRepository(IEventSourcingAggregateStore eventSourcingAggregateStore,
        IDomainEventMapper<PostIdentifier> domainEventMapper,
        IEventSourcingAggregateMapper<PostIdentifier> eventSourcingAggregateMapper,
        IDomainEventsPublisher<PostIdentifier> domainEventsPublisher) : base(eventSourcingAggregateStore,
        domainEventMapper, eventSourcingAggregateMapper, domainEventsPublisher)
    {
    }
}
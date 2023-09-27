using EssentialFrame.Domain.Core.Events.Services.Interfaces;
using EssentialFrame.Domain.Persistence.Mappers.Interfaces;
using EssentialFrame.Domain.Persistence.Services;
using EssentialFrame.Domain.Persistence.Services.Interfaces;
using EssentialFrame.ExampleApp.Domain.PostComments.Aggregates;
using EssentialFrame.ExampleApp.Domain.PostComments.Repositories;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Infrastructure.Repositories.PostComments;

internal sealed class PostCommentRepository : AggregateRepository<PostComment, PostCommentIdentifier>,
    IPostCommentRepository
{
    public PostCommentRepository(IAggregateStore aggregateStore,
        IAggregateMapper<PostCommentIdentifier> aggregateMapper,
        IDomainEventsPublisher<PostCommentIdentifier> domainEventsPublisher) : base(aggregateStore, aggregateMapper,
        domainEventsPublisher)
    {
    }
}
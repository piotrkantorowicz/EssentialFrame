using EssentialFrame.Domain.Events;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Models;
using EssentialFrame.Domain.ValueObjects.Core;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Mappers.Interfaces;

public interface IDomainEventMapper<TAggregateIdentifier> where TAggregateIdentifier : TypedGuidIdentifier
{
    DomainEventDataModel Map(IDomainEvent<TAggregateIdentifier> domainEvent);

    DomainEventDataModel Map(IDomainEvent<TAggregateIdentifier> domainEvent, ISerializer serializer);

    IReadOnlyCollection<DomainEventDataModel> Map(IEnumerable<IDomainEvent<TAggregateIdentifier>> domainEvents);

    IReadOnlyCollection<DomainEventDataModel> Map(IEnumerable<IDomainEvent<TAggregateIdentifier>> domainEvents,
        ISerializer serializer);

    IDomainEvent<TAggregateIdentifier> Map(DomainEventDataModel domainEventDataModel);

    IDomainEvent<TAggregateIdentifier> Map(DomainEventDataModel domainEventDataModel, ISerializer serializer);

    IReadOnlyCollection<IDomainEvent<TAggregateIdentifier>>
        Map(IEnumerable<DomainEventDataModel> domainEventDataModels);

    IReadOnlyCollection<IDomainEvent<TAggregateIdentifier>> Map(IEnumerable<DomainEventDataModel> domainEvents,
        ISerializer serializer);
}
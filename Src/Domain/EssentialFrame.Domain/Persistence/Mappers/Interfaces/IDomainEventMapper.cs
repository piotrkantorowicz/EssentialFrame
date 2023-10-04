using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.Persistence.Models;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.Persistence.Mappers.Interfaces;

public interface IDomainEventMapper<TAggregateIdentifier, TType> where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    DomainEventDataModel Map(IDomainEvent<TAggregateIdentifier, TType> domainEvent);

    DomainEventDataModel Map(IDomainEvent<TAggregateIdentifier, TType> domainEvent, ISerializer serializer);

    IReadOnlyCollection<DomainEventDataModel> Map(IEnumerable<IDomainEvent<TAggregateIdentifier, TType>> domainEvents);

    IReadOnlyCollection<DomainEventDataModel> Map(IEnumerable<IDomainEvent<TAggregateIdentifier, TType>> domainEvents,
        ISerializer serializer);

    IDomainEvent<TAggregateIdentifier, TType> Map(DomainEventDataModel domainEventDataModel);

    IDomainEvent<TAggregateIdentifier, TType> Map(DomainEventDataModel domainEventDataModel, ISerializer serializer);

    IReadOnlyCollection<IDomainEvent<TAggregateIdentifier, TType>> Map(
        IEnumerable<DomainEventDataModel> domainEventDataModels);

    IReadOnlyCollection<IDomainEvent<TAggregateIdentifier, TType>> Map(IEnumerable<DomainEventDataModel> domainEvents,
        ISerializer serializer);
}
using EssentialFrame.Domain.Events.Exceptions;
using EssentialFrame.Domain.Events.Persistence.DomainEvents.Interfaces;
using EssentialFrame.Extensions;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.Events.Persistence.DomainEvents;

public class DomainEventMapper : IDomainEventMapper
{
    public DomainEventDataModel Map(IDomainEvent domainEvent)
    {
        return new DomainEventDataModel
        {
            AggregateIdentifier = domainEvent.AggregateIdentifier,
            AggregateVersion = domainEvent.AggregateVersion,
            EventIdentifier = domainEvent.EventIdentifier,
            EventType = domainEvent.GetTypeFullName(),
            EventClass = domainEvent.GetClassName(),
            DomainEvent = domainEvent,
            CreatedAt = domainEvent.EventTime
        };
    }

    public DomainEventDataModel Map(IDomainEvent domainEvent, ISerializer serializer)
    {
        return new DomainEventDataModel
        {
            AggregateIdentifier = domainEvent.AggregateIdentifier,
            AggregateVersion = domainEvent.AggregateVersion,
            EventIdentifier = domainEvent.EventIdentifier,
            EventType = domainEvent.GetTypeFullName(),
            EventClass = domainEvent.GetClassName(),
            DomainEvent = serializer.Serialize(domainEvent),
            CreatedAt = domainEvent.EventTime
        };
    }

    public IReadOnlyCollection<DomainEventDataModel> Map(IEnumerable<IDomainEvent> domainEvents)
    {
        return domainEvents.Select(Map).ToList();
    }

    public IReadOnlyCollection<DomainEventDataModel> Map(IEnumerable<IDomainEvent> domainEvents, ISerializer serializer)
    {
        return domainEvents.Select(de => Map(de, serializer)).ToList();
    }

    public IDomainEvent Map(DomainEventDataModel domainEventDataModel)
    {
        return domainEventDataModel.DomainEvent as IDomainEvent;
    }

    public IDomainEvent Map(DomainEventDataModel domainEventDataModel, ISerializer serializer)
    {
        string serializedEvent = domainEventDataModel.DomainEvent as string;

        IDomainEvent deserialized =
            serializer.Deserialize<IDomainEvent>(serializedEvent, Type.GetType(domainEventDataModel.EventClass));

        if (deserialized is null)
        {
            throw new UnknownEventTypeException(domainEventDataModel.EventType);
        }

        return deserialized;
    }

    public IReadOnlyCollection<IDomainEvent> Map(IEnumerable<DomainEventDataModel> domainEventDataModels)
    {
        return domainEventDataModels.Select(Map).ToList();
    }

    public IReadOnlyCollection<IDomainEvent> Map(IEnumerable<DomainEventDataModel> domainEvents, ISerializer serializer)
    {
        return domainEvents.Select(de => Map(de, serializer)).ToList();
    }
}
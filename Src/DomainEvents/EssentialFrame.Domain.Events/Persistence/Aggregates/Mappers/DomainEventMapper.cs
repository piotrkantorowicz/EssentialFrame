﻿using EssentialFrame.Domain.Events.Exceptions;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Models;
using EssentialFrame.Domain.ValueObjects;
using EssentialFrame.Extensions;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers;

public class DomainEventMapper<TAggregateIdentifier> : IDomainEventMapper<TAggregateIdentifier>
    where TAggregateIdentifier : TypedGuidIdentifier
{
    public DomainEventDataModel Map(IDomainEvent<TAggregateIdentifier> domainEvent)
    {
        return new DomainEventDataModel
        {
            AggregateIdentifier = domainEvent.AggregateIdentifier.Identifier,
            AggregateVersion = domainEvent.AggregateVersion,
            EventIdentifier = domainEvent.EventIdentifier,
            EventType = domainEvent.GetTypeFullName(),
            EventClass = domainEvent.GetClassName(),
            DomainEvent = domainEvent,
            CreatedAt = domainEvent.EventTime
        };
    }

    public DomainEventDataModel Map(IDomainEvent<TAggregateIdentifier> domainEvent, ISerializer serializer)
    {
        return new DomainEventDataModel
        {
            AggregateIdentifier = domainEvent.AggregateIdentifier.Identifier,
            AggregateVersion = domainEvent.AggregateVersion,
            EventIdentifier = domainEvent.EventIdentifier,
            EventType = domainEvent.GetTypeFullName(),
            EventClass = domainEvent.GetClassName(),
            DomainEvent = serializer.Serialize(domainEvent),
            CreatedAt = domainEvent.EventTime
        };
    }

    public IReadOnlyCollection<DomainEventDataModel> Map(IEnumerable<IDomainEvent<TAggregateIdentifier>> domainEvents)
    {
        return domainEvents.Select(Map).ToList();
    }

    public IReadOnlyCollection<DomainEventDataModel> Map(IEnumerable<IDomainEvent<TAggregateIdentifier>> domainEvents,
        ISerializer serializer)
    {
        return domainEvents.Select(de => Map(de, serializer)).ToList();
    }

    public IDomainEvent<TAggregateIdentifier> Map(DomainEventDataModel domainEventDataModel)
    {
        return domainEventDataModel.DomainEvent as IDomainEvent<TAggregateIdentifier>;
    }

    public IDomainEvent<TAggregateIdentifier> Map(DomainEventDataModel domainEventDataModel, ISerializer serializer)
    {
        string serializedEvent = domainEventDataModel.DomainEvent as string;

        IDomainEvent<TAggregateIdentifier> deserialized =
            serializer.Deserialize<IDomainEvent<TAggregateIdentifier>>(serializedEvent,
                Type.GetType(domainEventDataModel.EventClass));

        if (deserialized is null)
        {
            throw new UnknownEventTypeException(domainEventDataModel.EventType);
        }

        return deserialized;
    }

    public IReadOnlyCollection<IDomainEvent<TAggregateIdentifier>> Map(
        IEnumerable<DomainEventDataModel> domainEventDataModels)
    {
        return domainEventDataModels.Select(Map).ToList();
    }

    public IReadOnlyCollection<IDomainEvent<TAggregateIdentifier>> Map(IEnumerable<DomainEventDataModel> domainEvents,
        ISerializer serializer)
    {
        return domainEvents.Select(de => Map(de, serializer)).ToList();
    }
}
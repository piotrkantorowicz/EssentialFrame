﻿using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Domain.Persistence.Mappers.Interfaces;
using EssentialFrame.Domain.Persistence.Models;
using EssentialFrame.Extensions;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.Persistence.Mappers;

internal sealed class DomainEventMapper<TAggregateIdentifier, TType> : IDomainEventMapper<TAggregateIdentifier, TType>
    where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    public DomainEventDataModel Map(IDomainEvent<TAggregateIdentifier, TType> domainEvent)
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

    public DomainEventDataModel Map(IDomainEvent<TAggregateIdentifier, TType> domainEvent, ISerializer serializer)
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

    public IReadOnlyCollection<DomainEventDataModel> Map(
        IEnumerable<IDomainEvent<TAggregateIdentifier, TType>> domainEvents)
    {
        return domainEvents.Select(Map).ToList();
    }

    public IReadOnlyCollection<DomainEventDataModel> Map(
        IEnumerable<IDomainEvent<TAggregateIdentifier, TType>> domainEvents,
        ISerializer serializer)
    {
        return domainEvents.Select(de => Map(de, serializer)).ToList();
    }

    public IDomainEvent<TAggregateIdentifier, TType> Map(DomainEventDataModel domainEventDataModel)
    {
        return domainEventDataModel.DomainEvent as IDomainEvent<TAggregateIdentifier, TType>;
    }

    public IDomainEvent<TAggregateIdentifier, TType> Map(DomainEventDataModel domainEventDataModel,
        ISerializer serializer)
    {
        string serializedEvent = domainEventDataModel.DomainEvent as string;

        IDomainEvent<TAggregateIdentifier, TType> deserialized =
            serializer.Deserialize<IDomainEvent<TAggregateIdentifier, TType>>(serializedEvent,
                Type.GetType(domainEventDataModel.EventClass));

        if (deserialized is null)
        {
            throw new UnknownEventTypeException(domainEventDataModel.EventType);
        }

        return deserialized;
    }

    public IReadOnlyCollection<IDomainEvent<TAggregateIdentifier, TType>> Map(
        IEnumerable<DomainEventDataModel> domainEventDataModels)
    {
        return domainEventDataModels.Select(Map).ToList();
    }

    public IReadOnlyCollection<IDomainEvent<TAggregateIdentifier, TType>> Map(
        IEnumerable<DomainEventDataModel> domainEvents,
        ISerializer serializer)
    {
        return domainEvents.Select(de => Map(de, serializer)).ToList();
    }
}
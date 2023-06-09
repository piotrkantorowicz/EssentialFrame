using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.Events.Persistence.DomainEvents.Interfaces;

public interface IDomainEventMapper
{
    DomainEventDataModel Map(IDomainEvent domainEvent);

    DomainEventDataModel Map(IDomainEvent domainEvent, ISerializer serializer);

    IReadOnlyCollection<DomainEventDataModel> Map(IEnumerable<IDomainEvent> domainEvents);

    IReadOnlyCollection<DomainEventDataModel> Map(IEnumerable<IDomainEvent> domainEvents, ISerializer serializer);

    IDomainEvent Map(DomainEventDataModel domainEventDataModel);

    IDomainEvent Map(DomainEventDataModel domainEventDataModel, ISerializer serializer);

    IReadOnlyCollection<IDomainEvent> Map(IEnumerable<DomainEventDataModel> domainEventDataModels);

    IReadOnlyCollection<IDomainEvent> Map(IEnumerable<DomainEventDataModel> domainEvents, ISerializer serializer);
}
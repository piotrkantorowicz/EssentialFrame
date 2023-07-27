using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Models;

namespace EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers.Interfaces;

public interface IAggregateMapper
{
    AggregateDataModel Map(AggregateRoot aggregateRoot);
}
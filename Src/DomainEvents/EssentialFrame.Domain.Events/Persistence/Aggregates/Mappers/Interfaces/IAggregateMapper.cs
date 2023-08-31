using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Events.Core.Aggregates;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Models;
using AggregateRoot = EssentialFrame.Domain.Events.Core.Aggregates.AggregateRoot;

namespace EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers.Interfaces;

public interface IAggregateMapper
{
    AggregateDataModel Map(AggregateRoot aggregateRoot);
}
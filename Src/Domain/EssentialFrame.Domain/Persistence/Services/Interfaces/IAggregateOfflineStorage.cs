using EssentialFrame.Domain.Persistence.Models;

namespace EssentialFrame.Domain.Persistence.Services.Interfaces;

public interface IAggregateOfflineStorage
{
    void Save(AggregateDataModel aggregate);

    Task SaveAsync(AggregateDataModel eventSourcingAggregate, CancellationToken cancellationToken = default);
}
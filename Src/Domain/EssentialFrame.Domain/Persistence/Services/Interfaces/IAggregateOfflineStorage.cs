using System.Text;
using EssentialFrame.Domain.Persistence.Models;

namespace EssentialFrame.Domain.Persistence.Services.Interfaces;

internal interface IAggregateOfflineStorage
{
    void Save(AggregateDataModel aggregate, Encoding encoding);

    Task SaveAsync(AggregateDataModel eventSourcingAggregate, Encoding encoding, CancellationToken cancellationToken);
}
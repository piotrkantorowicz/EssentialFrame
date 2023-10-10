using System.Text;
using EssentialFrame.Domain.Persistence.Models;

namespace EssentialFrame.Domain.Persistence.Services.Interfaces;

internal interface IAggregateOfflineStorage
{
    AggregateDataModel Get(string aggregateIdentifier, Encoding encoding);

    Task<AggregateDataModel> GetAsync(string aggregateIdentifier, Encoding encoding,
        CancellationToken cancellationToken);
    
    void Save(AggregateDataModel aggregate, Encoding encoding);

    Task SaveAsync(AggregateDataModel eventSourcingAggregate, Encoding encoding, CancellationToken cancellationToken);
}
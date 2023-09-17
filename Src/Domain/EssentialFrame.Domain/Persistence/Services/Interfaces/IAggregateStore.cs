using EssentialFrame.Domain.Persistence.Models;

namespace EssentialFrame.Domain.Persistence.Services.Interfaces;

public interface IAggregateStore
{
    bool Exists(Guid aggregateIdentifier);

    Task<bool> ExistsAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default);

    AggregateDataModel Get(Guid aggregateIdentifier);

    Task<AggregateDataModel> GetAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default);

    IEnumerable<Guid> GetExpired();

    Task<IEnumerable<Guid>> GetExpiredAsync(CancellationToken cancellationToken = default);

    void Save(AggregateDataModel aggregate);

    Task SaveAsync(AggregateDataModel aggregate, CancellationToken cancellationToken = default);

    void Box(Guid aggregateIdentifier);

    Task BoxAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default);
}
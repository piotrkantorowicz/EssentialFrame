using EssentialFrame.Domain.Persistence.Models;

namespace EssentialFrame.Domain.Persistence.Services.Interfaces;

public interface IAggregateStore
{
    bool Exists(string aggregateIdentifier);

    Task<bool> ExistsAsync(string aggregateIdentifier, CancellationToken cancellationToken = default);

    AggregateDataModel Get(string aggregateIdentifier);

    Task<AggregateDataModel> GetAsync(string aggregateIdentifier, CancellationToken cancellationToken = default);

    IEnumerable<string> GetExpired();

    Task<IEnumerable<string>> GetExpiredAsync(CancellationToken cancellationToken = default);

    void Save(AggregateDataModel aggregate);

    Task SaveAsync(AggregateDataModel aggregate, CancellationToken cancellationToken = default);

    void Box(string aggregateIdentifier);

    Task BoxAsync(string aggregateIdentifier, CancellationToken cancellationToken = default);
}
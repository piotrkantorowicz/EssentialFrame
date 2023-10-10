using System.Text;
using EssentialFrame.Domain.Core.Aggregates;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.Persistence.Services.Interfaces;

public interface IAggregateRepository<TAggregate, TAggregateIdentifier, TType>
    where TAggregate : class, IAggregateRoot<TAggregateIdentifier, TType>
    where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    TAggregate Get(TAggregateIdentifier aggregateIdentifier);

    TAggregate Get(TAggregateIdentifier aggregateIdentifier, ISerializer serializer);

    Task<TAggregate> GetAsync(TAggregateIdentifier aggregateIdentifier, CancellationToken cancellationToken);

    Task<TAggregate> GetAsync(TAggregateIdentifier aggregateIdentifier, ISerializer serializer,
        CancellationToken cancellationToken);

    void Save(TAggregate aggregate);

    Task SaveAsync(TAggregate aggregate, CancellationToken cancellationToken);

    void Box(TAggregateIdentifier aggregateIdentifier);

    void Box(TAggregateIdentifier aggregateIdentifier, Encoding encoding);

    Task BoxAsync(TAggregateIdentifier aggregateIdentifier, CancellationToken cancellationToken);

    Task BoxAsync(TAggregateIdentifier aggregateIdentifier, Encoding encoding, CancellationToken cancellationToken);

    TAggregate Unbox(TAggregateIdentifier aggregateIdentifier);

    TAggregate Unbox(TAggregateIdentifier aggregateIdentifier, Encoding encoding);

    Task<TAggregate> UnboxAsync(TAggregateIdentifier aggregateIdentifier, CancellationToken cancellationToken);

    Task<TAggregate> UnboxAsync(TAggregateIdentifier aggregateIdentifier, Encoding encoding,
        CancellationToken cancellationToken);
}
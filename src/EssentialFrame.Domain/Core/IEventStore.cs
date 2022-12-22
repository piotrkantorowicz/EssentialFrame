using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Serialization;

namespace EssentialFrame.Domain.Core;

public interface IEventStore
{
    ISerializer Serializer { get; }

    bool Exists(Guid aggregate);

    bool Exists(Guid aggregate, int version);

    IEnumerable<IEvent> Get(Guid aggregate, int version);

    IEnumerable<Guid> GetExpired(DateTimeOffset at);

    void Save(AggregateRoot aggregate, IEnumerable<IEvent> events);

    void Box(Guid aggregate);
}





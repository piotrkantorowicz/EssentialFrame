using EssentialFrame.Domain.Aggregates;

namespace EssentialFrame.Domain.Core;

public interface IEventRepository
{
    T Get<T>(Guid id)
        where T : AggregateRoot;

    IEvent[] Save<T>(T aggregate, int? version = null)
        where T : AggregateRoot;

    void Box<T>(T aggregate)
        where T : AggregateRoot;

    T Unbox<T>(Guid aggregate)
        where T : AggregateRoot;
}

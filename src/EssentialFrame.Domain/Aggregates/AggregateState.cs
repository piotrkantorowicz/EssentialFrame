using EssentialFrame.Domain.Core;
using EssentialFrame.Domain.Exceptions;

namespace EssentialFrame.Domain.Aggregates;

public abstract class AggregateState
{
    public void Apply(IEvent @event)
    {
        var when = GetType()
            .GetMethod("When",
                       new[]
                       {
                           @event.GetType()
                       });

        if (when == null)
        {
            throw new MethodNotFoundException(GetType(),
                                              "When",
                                              @event.GetType());
        }

        when.Invoke(this,
                    new object[]
                    {
                        @event
                    });
    }
}


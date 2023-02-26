using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Exceptions;

public class DomainEventDoesNotMatchException : EssentialFrameException
{
    public DomainEventDoesNotMatchException(Guid aggregate, Guid expectedAggregate) : base(
        $"The event is not match aggregate ({expectedAggregate}), because was assigned to other aggregate ({aggregate}).")
    {
    }
}
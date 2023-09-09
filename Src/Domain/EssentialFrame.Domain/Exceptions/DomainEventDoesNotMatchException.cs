using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Exceptions;

public class DomainEventDoesNotMatchException : EssentialFrameException
{
    public DomainEventDoesNotMatchException(string aggregateIdentifier, string expectedAggregateIdentifier) : base(
        $"The event is not match aggregate ({expectedAggregateIdentifier}), because was assigned to other aggregate ({aggregateIdentifier})")
    {
    }
}
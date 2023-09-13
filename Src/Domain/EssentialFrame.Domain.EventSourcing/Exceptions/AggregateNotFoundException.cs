using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.EventSourcing.Exceptions;

[Serializable]
internal class AggregateNotFoundException : EssentialFrameException
{
    public AggregateNotFoundException(Type type, string aggregateIdentifier) : base(
        $"This aggregate does not exist ({type.FullName} {aggregateIdentifier}) because there are no events for it")
    {
    }
}
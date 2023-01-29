using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Events.Exceptions;

[Serializable]
internal class AggregateNotFoundException : EssentialFrameException
{
    public AggregateNotFoundException(Type type, Guid id) : base(
        $"This aggregate does not exist ({type.FullName} {id}) because there are no events for it.")
    {
    }
}
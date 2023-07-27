using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Events.Exceptions;

[Serializable]
internal class AggregateDeletedException : EssentialFrameException
{
    public AggregateDeletedException(Guid aggregateIdentifier, Type aggregateType) : base(
        $"Unable to get aggregate ({aggregateType.FullName}) with id: ({aggregateIdentifier}), because it is deleted.")
    {
    }
}
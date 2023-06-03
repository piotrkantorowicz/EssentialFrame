using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Events.Exceptions;

[Serializable]
internal class SnapshotBoxingFailedException : EssentialFrameException
{
    private SnapshotBoxingFailedException(string message) : base(message)
    {
    }

    private SnapshotBoxingFailedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public static SnapshotBoxingFailedException Unexpected(Guid aggregateIdentifier, Exception innerException)
    {
        return new SnapshotBoxingFailedException(
            $"Unexpected error while boxing snapshot for aggregate with id: ({aggregateIdentifier}). See inner exception for more details.",
            innerException);
    }

    public static SnapshotBoxingFailedException SnapshotNotFound(Guid aggregateIdentifier)
    {
        return new SnapshotBoxingFailedException(
            $"Unable to box snapshot for aggregate with id: ({aggregateIdentifier}), because snapshot hasn't been found. See inner exception for more details.");
    }
}
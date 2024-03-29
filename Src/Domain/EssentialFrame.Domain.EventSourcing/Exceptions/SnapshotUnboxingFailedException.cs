﻿using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.EventSourcing.Exceptions;

[Serializable]
internal class SnapshotUnboxingFailedException : EssentialFrameException
{
    private SnapshotUnboxingFailedException(string message) : base(message)
    {
    }

    private SnapshotUnboxingFailedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public static SnapshotUnboxingFailedException Unexpected(string aggregateIdentifier, Exception innerException)
    {
        return new SnapshotUnboxingFailedException(
            $"Unexpected error while unboxing snapshot for aggregate with id: ({aggregateIdentifier}). See inner exception for more details",
            innerException);
    }

    public static SnapshotUnboxingFailedException SnapshotNotFound(string aggregateIdentifier)
    {
        return new SnapshotUnboxingFailedException(
            $"Unable to unbox snapshot for aggregate with id: ({aggregateIdentifier}), because snapshot hasn't been found. See inner exception for more details");
    }
}
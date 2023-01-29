using System;
using System.Runtime.Serialization;

namespace EssentialFrame.Exceptions;

public class EssentialFrameException : Exception
{
    protected EssentialFrameException(string message) : base(message)
    {
    }

    protected EssentialFrameException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected EssentialFrameException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
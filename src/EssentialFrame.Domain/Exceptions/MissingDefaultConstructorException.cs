using System.Runtime.Serialization;

namespace EssentialFrame.Domain.Exceptions;

[Serializable]
internal class MissingDefaultConstructorException : Exception
{
    public MissingDefaultConstructorException(Type type)
        : base($"This class has no default constructor ({type.FullName}).")
    {
    }

    protected MissingDefaultConstructorException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Exceptions;

[Serializable]
internal class MissingDefaultConstructorException : EssentialFrameException
{
    public MissingDefaultConstructorException(Type type) : base(
        $"This class has no default constructor ({type.FullName}).")
    {
    }
}
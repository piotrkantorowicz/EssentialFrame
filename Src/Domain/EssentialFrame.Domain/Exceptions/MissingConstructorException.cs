using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Exceptions;

[Serializable]
public class MissingConstructorException : EssentialFrameException
{
    public MissingConstructorException(Type type) : base(
        $"This class has no constructor ({type.FullName}) that can be used to create an aggregate.")
    {
    }
}
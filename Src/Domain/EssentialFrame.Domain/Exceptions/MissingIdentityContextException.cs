using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Exceptions;

[Serializable]
public class MissingIdentityContextException : EssentialFrameException
{
    public MissingIdentityContextException(Type aggregateType) : base(
        $"This aggregate ({aggregateType.FullName}) has missing identity context. Consider to build your aggregates via constructor allows to pass an identity parameter")
    {
    }
}
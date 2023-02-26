using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Exceptions;

[Serializable]
public class MethodNotFoundException : EssentialFrameException
{
    public MethodNotFoundException(Type classType, string methodName, Type parameterType) : base(
        $"This class ({classType.FullName}) has no method named ({methodName}) that takes this parameter ({parameterType}).")
    {
    }
}
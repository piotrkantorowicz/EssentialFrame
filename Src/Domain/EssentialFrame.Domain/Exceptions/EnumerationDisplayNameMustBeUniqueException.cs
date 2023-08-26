using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Exceptions;

[Serializable]
public class EnumerationDisplayNameMustBeUniqueException : EssentialFrameException
{
    public EnumerationDisplayNameMustBeUniqueException(Type classType, string displayName) : base(
        $"This class ({classType.FullName}) has already an enumeration with this display name ({displayName})")
    {
    }
}
namespace EssentialFrame.Core.Extensions;

public static class TypeExtensions
{
    public static string GetTypeName(this object @object)
    {
        return @object.GetType().Name;
    }
}

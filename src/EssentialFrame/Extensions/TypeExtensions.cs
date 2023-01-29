namespace EssentialFrame.Extensions;

public static class TypeExtensions
{
    public static string GetTypeName(this object @object)
    {
        return @object.GetType().Name;
    }

    public static string GetTypeFullName(this object @object)
    {
        return @object.GetType().FullName;
    }

    public static string GetClassName(this object @object)
    {
        return @object.GetType().AssemblyQualifiedName;
    }
}
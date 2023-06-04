namespace EssentialFrame.Extensions;

public static class TypeExtensions
{
    public static string GetTypeName<T>(this T @object)
    {
        return @object.GetType().Name;
    }

    public static string GetTypeFullName<T>(this T @object)
    {
        return @object.GetType().FullName;
    }

    public static string GetClassName<T>(this T @object)
    {
        return @object.GetType().AssemblyQualifiedName;
    }
}
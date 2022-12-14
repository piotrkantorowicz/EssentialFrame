namespace EssentialFrame.Core.Extensions;

public static class TypeExtensions
{
    public static string GetTypeName(this object @object) => @object.GetType().Name;

    public static string GetTypeFullName(this object @object) => @object.GetType().FullName;

    public static string GetClassName(this object @object) => @object.GetType().AssemblyQualifiedName;
}

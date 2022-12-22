using Autofac;

namespace EssentialFrame.Serialization.SpanJson.Autofac;

internal static class AutofacSpanJsonSerializerRegistration
{
    public static IEssentialFrameBuilder<ContainerBuilder, IContainer> AddSpanJsonSerializer(
        this IEssentialFrameBuilder<ContainerBuilder, IContainer> essentialFrameBuilder)
    {
        var containerBuilder = essentialFrameBuilder.Builder;

        containerBuilder.RegisterType<SpanJsonSerializer>()
                        .As<ISerializer>()
                        .InstancePerLifetimeScope();

        return essentialFrameBuilder;
    }
}

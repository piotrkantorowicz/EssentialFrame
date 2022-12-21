using Autofac;

namespace EssentialFrame.Serialization.SpanJson.Autofac;

public static class AutofacRegistration
{
    public static ContainerBuilder AddCqrsCommandsLogging(this ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterType<SpanJsonSerializer>()
                        .As<ISerializer>()
                        .InstancePerLifetimeScope();

        return containerBuilder;
    }
}

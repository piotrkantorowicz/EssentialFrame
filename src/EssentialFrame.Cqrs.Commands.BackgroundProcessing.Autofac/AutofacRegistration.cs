using Autofac;
using EssentialFrame.Cqrs.Commands.BackgroundProcessing.Autofac.Services;

namespace EssentialFrame.Cqrs.Commands.BackgroundProcessing.Autofac;

public static class AutofacRegistration
{
    public static ContainerBuilder AddCommandsBackgroundProcessor(this ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterType<AutofacCommandBackgroundService>()
                        .AsImplementedInterfaces()
                        .SingleInstance();

        return containerBuilder;
    }
}

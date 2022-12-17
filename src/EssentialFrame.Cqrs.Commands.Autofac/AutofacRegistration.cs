using System.Reflection;
using Autofac;
using EssentialFrame.Cqrs.Commands.Autofac.Executors;
using EssentialFrame.Cqrs.Commands.Interfaces;

namespace EssentialFrame.Cqrs.Commands.Autofac;

public static class AutofacRegistration
{
    public static ContainerBuilder AddCommandHandlers(this ContainerBuilder containerBuilder,
                                                      Assembly[] assemblies)
    {
        containerBuilder.RegisterAssemblyTypes(assemblies)
                        .AsClosedTypesOf(typeof(ICommandHandler<>))
                        .InstancePerLifetimeScope();

        containerBuilder.RegisterAssemblyTypes(assemblies)
                        .AsClosedTypesOf(typeof(IAsyncCommandHandler<>))
                        .InstancePerLifetimeScope();

        return containerBuilder;
    }

    public static ContainerBuilder AddCommandExecutors(this ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterType<AutofacCommandExecutor>()
                        .As<ICommandExecutor>()
                        .InstancePerLifetimeScope();

        return containerBuilder;
    }
}


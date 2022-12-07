using System.Reflection;
using Autofac;
using EssentialFrame.Cqrs.Autofac.Dispatchers;
using EssentialFrame.Cqrs.Interfaces;

namespace EssentialFrame.Cqrs.Autofac;

public static class CqrsRegistration
{
    public static ContainerBuilder AddCqrs(this ContainerBuilder containerBuilder,
                                           Assembly[] assemblies)
    {
        containerBuilder.RegisterAssemblyTypes(assemblies)
                        .AsClosedTypesOf(typeof(ICommandHandler<>))
                        .InstancePerLifetimeScope();

        containerBuilder.RegisterAssemblyTypes(assemblies)
                        .AsClosedTypesOf(typeof(IQueryHandler<,>))
                        .InstancePerLifetimeScope();

        containerBuilder.RegisterType<CommandDispatcher>()
                        .As<ICommandDispatcher>()
                        .InstancePerLifetimeScope();

        containerBuilder.RegisterType<QueryDispatcher>()
                        .As<IQueryDispatcher>()
                        .InstancePerLifetimeScope();

        return containerBuilder;
    }
}

using System.Reflection;
using Autofac;
using EssentialFrame.Cqrs.Autofac.Dispatchers;
using EssentialFrame.Cqrs.Autofac.Services;
using EssentialFrame.Cqrs.Interfaces;

namespace EssentialFrame.Cqrs.Autofac;

public static class AutofacCqrsRegistration
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

        containerBuilder.RegisterType<AutofacCommandDispatcher>()
                        .As<ICommandDispatcher>()
                        .InstancePerLifetimeScope();

        containerBuilder.RegisterType<AutofacQueryDispatcher>()
                        .As<IQueryDispatcher>()
                        .InstancePerLifetimeScope();

        containerBuilder.RegisterType<AutofacCommandBackgroundService>()
                        .AsImplementedInterfaces()
                        .SingleInstance();


        return containerBuilder;
    }
}

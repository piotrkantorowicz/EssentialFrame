using System.Reflection;
using Autofac;
using EssentialFrame.Cqrs.Queries.Autofac.Executors;
using EssentialFrame.Cqrs.Queries.Interfaces;

namespace EssentialFrame.Cqrs.Queries.Autofac;

public static class AutofacRegistration
{
    public static ContainerBuilder AddQueryHandlers(this ContainerBuilder containerBuilder,
                                                    Assembly[] assemblies)
    {
        containerBuilder.RegisterAssemblyTypes(assemblies)
                        .AsClosedTypesOf(typeof(IQueryHandler<,>))
                        .InstancePerLifetimeScope();

        containerBuilder.RegisterAssemblyTypes(assemblies)
                        .AsClosedTypesOf(typeof(IAsyncQueryHandler<,>))
                        .InstancePerLifetimeScope();

        return containerBuilder;
    }

    public static ContainerBuilder AddQueryExecutors(this ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterType<AutofacQueryExecutor>()
                        .As<IQueryExecutor>()
                        .InstancePerLifetimeScope();

        return containerBuilder;
    }
}

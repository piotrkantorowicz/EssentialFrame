using System.Reflection;
using Autofac;
using EssentialFrame.Cqrs.Queries.Core.Interfaces;
using EssentialFrame.Cqrs.Queries.Logging.Decorators;
using EssentialFrame.Cqrs.Queries.Services.Execution;
using EssentialFrame.Cqrs.Queries.Services.Execution.Interfaces;

namespace EssentialFrame.Cqrs.Queries.Di;

internal static class QueriesRegistration
{
    public static IEssentialFrameBuilder<ContainerBuilder, IContainer> AddQueriesHandlers(
        this IEssentialFrameBuilder<ContainerBuilder, IContainer> essentialFrameBuilder, Assembly[] assemblies)
    {
        ContainerBuilder containerBuilder = essentialFrameBuilder.Builder;

        containerBuilder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(IQueryHandler<,>))
            .InstancePerLifetimeScope();

        containerBuilder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(IAsyncQueryHandler<,>))
            .InstancePerLifetimeScope();

        return essentialFrameBuilder;
    }

    public static IEssentialFrameBuilder<ContainerBuilder, IContainer> AddQueriesExecutors(
        this IEssentialFrameBuilder<ContainerBuilder, IContainer> essentialFrameBuilder)
    {
        ContainerBuilder containerBuilder = essentialFrameBuilder.Builder;

        containerBuilder.RegisterType<DefaultQueryExecutor>().As<IQueryExecutor>().InstancePerLifetimeScope();

        return essentialFrameBuilder;
    }

    public static IEssentialFrameBuilder<ContainerBuilder, IContainer> AddQueriesLogging(
        this IEssentialFrameBuilder<ContainerBuilder, IContainer> essentialFrameBuilder)
    {
        ContainerBuilder containerBuilder = essentialFrameBuilder.Builder;

        containerBuilder.RegisterGenericDecorator(typeof(LoggingAsyncQueryHandlerDecorator<,>),
            typeof(IAsyncQueryHandler<,>));

        containerBuilder.RegisterGenericDecorator(typeof(LoggingQueryHandlerDecorator<,>), typeof(IQueryHandler<,>));

        return essentialFrameBuilder;
    }
}
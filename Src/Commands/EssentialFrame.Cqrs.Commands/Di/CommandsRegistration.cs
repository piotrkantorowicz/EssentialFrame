using System.Reflection;
using Autofac;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Logging.Decorators;
using EssentialFrame.Cqrs.Commands.Persistence;
using EssentialFrame.Cqrs.Commands.Persistence.Interfaces;
using EssentialFrame.Cqrs.Commands.Services.Execution;
using EssentialFrame.Cqrs.Commands.Services.Execution.Interfaces;
using EssentialFrame.Cqrs.Commands.Services.Scheduling;

namespace EssentialFrame.Cqrs.Commands.Di;

public static class CommandsRegistration
{
    public static IEssentialFrameBuilder<ContainerBuilder, IContainer> AddCommandsInfrastructure(
        this IEssentialFrameBuilder<ContainerBuilder, IContainer> essentialFrameBuilder)
    {
        ContainerBuilder containerBuilder = essentialFrameBuilder.Builder;

        containerBuilder.RegisterType<CommandRepository>().As<ICommandRepository>().InstancePerLifetimeScope();
        containerBuilder.RegisterType<CommandDataModelService>().As<ICommandDataModelService>().SingleInstance();
        containerBuilder.RegisterType<CommandMapper>().As<ICommandMapper>().SingleInstance();

        return essentialFrameBuilder;
    }

    public static IEssentialFrameBuilder<ContainerBuilder, IContainer> AddCommandsHandlers(
        this IEssentialFrameBuilder<ContainerBuilder, IContainer> essentialFrameBuilder, Assembly[] assemblies)
    {
        ContainerBuilder containerBuilder = essentialFrameBuilder.Builder;

        containerBuilder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(ICommandHandler<>))
            .InstancePerLifetimeScope();

        containerBuilder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(IAsyncCommandHandler<>))
            .InstancePerLifetimeScope();

        return essentialFrameBuilder;
    }

    public static IEssentialFrameBuilder<ContainerBuilder, IContainer> AddDefaultCommandsExecutor(
        this IEssentialFrameBuilder<ContainerBuilder, IContainer> essentialFrameBuilder)
    {
        ContainerBuilder containerBuilder = essentialFrameBuilder.Builder;

        containerBuilder.RegisterType<DefaultCommandExecutor>().As<ICommandExecutor>().InstancePerLifetimeScope();

        return essentialFrameBuilder;
    }

    public static IEssentialFrameBuilder<ContainerBuilder, IContainer> AddDefaultCommandsScheduler(
        this IEssentialFrameBuilder<ContainerBuilder, IContainer> essentialFrameBuilder)
    {
        ContainerBuilder containerBuilder = essentialFrameBuilder.Builder;

        containerBuilder.RegisterType<DefaultCommandExecutor>().As<ICommandScheduler>().InstancePerLifetimeScope();

        return essentialFrameBuilder;
    }

    public static IEssentialFrameBuilder<ContainerBuilder, IContainer> AddDefaultCommandsBackgroundProcessor(
        this IEssentialFrameBuilder<ContainerBuilder, IContainer> essentialFrameBuilder, int interval)
    {
        ContainerBuilder containerBuilder = essentialFrameBuilder.Builder;

        containerBuilder.Register(ctx => new DefaultCommandsScheduler(ctx.Resolve<ILifetimeScope>(), interval))
            .AsImplementedInterfaces().SingleInstance();

        return essentialFrameBuilder;
    }

    public static IEssentialFrameBuilder<ContainerBuilder, IContainer> AddCqrsCommandsLogging(
        this IEssentialFrameBuilder<ContainerBuilder, IContainer> essentialFrameBuilder)
    {
        ContainerBuilder containerBuilder = essentialFrameBuilder.Builder;

        containerBuilder.RegisterGenericDecorator(typeof(LoggingAsyncCommandHandlerDecorator<>),
            typeof(IAsyncCommandHandler<>));

        containerBuilder.RegisterGenericDecorator(typeof(LoggingCommandHandlerDecorator<>), typeof(ICommandHandler<>));

        return essentialFrameBuilder;
    }
}
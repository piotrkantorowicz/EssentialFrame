using System.Reflection;
using Autofac;
using EssentialFrame.Cqrs.Commands.Autofac.Executors;
using EssentialFrame.Cqrs.Commands.Autofac.Services;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Logging.Decorators;
using EssentialFrame.Cqrs.Commands.Validations.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Validations.Decorators;

namespace EssentialFrame.Cqrs.Commands.Autofac;

internal static class AutofacCqrsCommandsRegistration
{
    public static IEssentialFrameBuilder<ContainerBuilder, IContainer> AddCommandsInfrastructure(
        this IEssentialFrameBuilder<ContainerBuilder, IContainer> essentialFrameBuilder)
    {
        var containerBuilder = essentialFrameBuilder.Builder;

        containerBuilder.RegisterType<CommandRepository>().As<ICommandRepository>().InstancePerLifetimeScope();

        return essentialFrameBuilder;
    }

    public static IEssentialFrameBuilder<ContainerBuilder, IContainer> AddCommandsHandlers(
        this IEssentialFrameBuilder<ContainerBuilder, IContainer> essentialFrameBuilder,
        Assembly[] assemblies)
    {
        var containerBuilder = essentialFrameBuilder.Builder;

        containerBuilder.RegisterAssemblyTypes(assemblies)
                        .AsClosedTypesOf(typeof(ICommandHandler<>))
                        .InstancePerLifetimeScope();

        containerBuilder.RegisterAssemblyTypes(assemblies)
                        .AsClosedTypesOf(typeof(IAsyncCommandHandler<>))
                        .InstancePerLifetimeScope();

        return essentialFrameBuilder;
    }

    public static IEssentialFrameBuilder<ContainerBuilder, IContainer> AddCommandsExecutor(
        this IEssentialFrameBuilder<ContainerBuilder, IContainer> essentialFrameBuilder)
    {
        var containerBuilder = essentialFrameBuilder.Builder;

        containerBuilder.RegisterType<AutofacCommandExecutor>().As<ICommandExecutor>().InstancePerLifetimeScope();

        return essentialFrameBuilder;
    }

    public static IEssentialFrameBuilder<ContainerBuilder, IContainer> AddCommandsScheduler(
        this IEssentialFrameBuilder<ContainerBuilder, IContainer> essentialFrameBuilder)
    {
        var containerBuilder = essentialFrameBuilder.Builder;

        containerBuilder.RegisterType<AutofacCommandExecutor>().As<ICommandScheduler>().InstancePerLifetimeScope();

        return essentialFrameBuilder;
    }

    public static IEssentialFrameBuilder<ContainerBuilder, IContainer> AddCommandsBackgroundProcessor(
        this IEssentialFrameBuilder<ContainerBuilder, IContainer> essentialFrameBuilder,
        int interval)
    {
        var containerBuilder = essentialFrameBuilder.Builder;

        containerBuilder.Register(ctx => new AutofacCommandBackgroundService(ctx.Resolve<ILifetimeScope>(), interval))
                        .AsImplementedInterfaces()
                        .SingleInstance();

        return essentialFrameBuilder;
    }

    public static IEssentialFrameBuilder<ContainerBuilder, IContainer> AddCqrsCommandsLogging(
        this IEssentialFrameBuilder<ContainerBuilder, IContainer> essentialFrameBuilder)
    {
        var containerBuilder = essentialFrameBuilder.Builder;

        containerBuilder.RegisterGenericDecorator(typeof(LoggingAsyncCommandHandlerDecorator<>),
                                                  typeof(IAsyncCommandHandler<>));

        containerBuilder.RegisterGenericDecorator(typeof(LoggingCommandHandlerDecorator<>), typeof(ICommandHandler<>));

        return essentialFrameBuilder;
    }

    public static IEssentialFrameBuilder<ContainerBuilder, IContainer> AddCommandsValidation(
        this IEssentialFrameBuilder<ContainerBuilder, IContainer> essentialFrameBuilder,
        Assembly[] assemblies)
    {
        var containerBuilder = essentialFrameBuilder.Builder;

        containerBuilder.RegisterAssemblyTypes(assemblies)
                        .AsClosedTypesOf(typeof(ICommandValidator<>))
                        .InstancePerLifetimeScope();

        containerBuilder.RegisterGenericDecorator(typeof(ValidationAsyncCommandHandlerDecorator<>),
                                                  typeof(IAsyncCommandHandler<>));

        containerBuilder.RegisterGenericDecorator(typeof(ValidationCommandHandlerDecorator<>),
                                                  typeof(ICommandHandler<>));

        return essentialFrameBuilder;
    }
}

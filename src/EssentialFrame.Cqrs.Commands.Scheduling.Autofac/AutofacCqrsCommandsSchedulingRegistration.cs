using Autofac;
using EssentialFrame.Cqrs.Commands.Scheduling.Autofac.Executors;
using EssentialFrame.Cqrs.Commands.Scheduling.Autofac.Schedulers;
using EssentialFrame.Cqrs.Commands.Scheduling.Autofac.Services;
using EssentialFrame.Cqrs.Commands.Scheduling.Core;

namespace EssentialFrame.Cqrs.Commands.Scheduling.Autofac;

public static class AutofacCqrsCommandsSchedulingRegistration
{
    public static IEssentialFrameBuilder<ContainerBuilder, IContainer> AddStoreCommandsExecutors(
        this IEssentialFrameBuilder<ContainerBuilder, IContainer> essentialFrameBuilder)
    {
        var containerBuilder = essentialFrameBuilder.Builder;

        containerBuilder.RegisterType<AutofacCommandStoreExecutor>()
                        .As<ICommandStoreExecutor>()
                        .InstancePerLifetimeScope();

        containerBuilder.RegisterType<AutofacCommandScheduler>()
                        .As<ICommandScheduler>()
                        .InstancePerLifetimeScope();

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
}

using Autofac;
using EssentialFrame.Domain.EventsSourcing.Events;
using EssentialFrame.Domain.EventsSourcing.Events.Interfaces;
using EssentialFrame.Domain.EventsSourcing.Snapshots;
using EssentialFrame.Domain.EventsSourcing.Snapshots.Interfaces;

namespace EssentialFrame.Domain.EventsSourcing.Autofac;

internal static class AutofacCqrsCommandsRegistration
{
    public static IEssentialFrameBuilder<ContainerBuilder, IContainer> AddEventsSourcingInfrastructure(
        this IEssentialFrameBuilder<ContainerBuilder, IContainer> essentialFrameBuilder)
    {
        var containerBuilder = essentialFrameBuilder.Builder;

        containerBuilder.RegisterType<EventRepository>()
                        .As<IEventRepository>()
                        .InstancePerLifetimeScope();

        containerBuilder.RegisterType<SnapshotRepository>()
                        .As<ISnapshotRepository>()
                        .InstancePerLifetimeScope();

        containerBuilder.RegisterType<SnapshotStrategy>()
                        .As<ISnapshotStrategy>()
                        .InstancePerLifetimeScope();

        containerBuilder.RegisterType<SnapshotStrategy>()
                        .As<ISnapshotStrategy>()
                        .InstancePerLifetimeScope();

        return essentialFrameBuilder;
    }
}

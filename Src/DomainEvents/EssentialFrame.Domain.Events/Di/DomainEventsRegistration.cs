using Autofac;
using EssentialFrame.Domain.Events.Core;
using EssentialFrame.Domain.Events.Core.Interfaces;
using EssentialFrame.Domain.Events.Persistence.DomainEvents;
using EssentialFrame.Domain.Events.Persistence.DomainEvents.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Snapshots;
using EssentialFrame.Domain.Events.Persistence.Snapshots.Interfaces;

namespace EssentialFrame.Domain.Events.Di;

internal static class DomainEventsRegistration
{
    public static IEssentialFrameBuilder<ContainerBuilder, IContainer> AddEventsSourcingInfrastructure(
        this IEssentialFrameBuilder<ContainerBuilder, IContainer> essentialFrameBuilder)
    {
        ContainerBuilder containerBuilder = essentialFrameBuilder.Builder;

        containerBuilder.RegisterType<DomainEventsRepository>().As<IDomainEventsRepository>()
            .InstancePerLifetimeScope();

        containerBuilder.RegisterType<SnapshotRepository>().As<ISnapshotRepository>().InstancePerLifetimeScope();
        containerBuilder.RegisterType<SnapshotStrategy>().As<ISnapshotStrategy>().InstancePerLifetimeScope();

        return essentialFrameBuilder;
    }
}
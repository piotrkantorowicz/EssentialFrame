using Autofac;
using EssentialFrame.Domain.Events.Core.Snapshots;
using EssentialFrame.Domain.Events.Core.Snapshots.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Services;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Snapshots.Services;
using EssentialFrame.Domain.Events.Persistence.Snapshots.Services.Interfaces;

namespace EssentialFrame.Domain.Events.Di;

internal static class DomainEventsRegistration
{
    public static IEssentialFrameBuilder<ContainerBuilder, IContainer> AddEventsSourcingInfrastructure(
        this IEssentialFrameBuilder<ContainerBuilder, IContainer> essentialFrameBuilder)
    {
        ContainerBuilder containerBuilder = essentialFrameBuilder.Builder;

        containerBuilder.RegisterType<AggregateRepository>().As<IAggregateRepository>().InstancePerLifetimeScope();
        containerBuilder.RegisterType<SnapshotRepository>().As<ISnapshotRepository>().InstancePerLifetimeScope();
        containerBuilder.RegisterType<SnapshotStrategy>().As<ISnapshotStrategy>().InstancePerLifetimeScope();

        return essentialFrameBuilder;
    }
}
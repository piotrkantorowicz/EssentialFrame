using System.Reflection;
using Autofac;
using EssentialFrame.Domain.EventSourcing.Core.Snapshots.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Services.Interfaces;

namespace EssentialFrame.Domain.EventSourcing.Di;

internal static class DomainEventsRegistration
{
    public static IEssentialFrameBuilder<ContainerBuilder, IContainer> AddEventsSourcingInfrastructure(
        this IEssentialFrameBuilder<ContainerBuilder, IContainer> essentialFrameBuilder, Assembly[] assemblies)
    {
        ContainerBuilder containerBuilder = essentialFrameBuilder.Builder;

        containerBuilder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(IEventSourcingAggregateRepository<,>))
            .InstancePerLifetimeScope();

        containerBuilder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(ISnapshotRepository<,>))
            .InstancePerLifetimeScope();

        containerBuilder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(ISnapshotStrategy<,>))
            .InstancePerLifetimeScope();

        return essentialFrameBuilder;
    }
}
using System.Reflection;
using Autofac;
using EssentialFrame.Domain.Events.Core.Snapshots.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Snapshots.Services.Interfaces;

namespace EssentialFrame.Domain.Events.Di;

internal static class DomainEventsRegistration
{
    public static IEssentialFrameBuilder<ContainerBuilder, IContainer> AddEventsSourcingInfrastructure(
        this IEssentialFrameBuilder<ContainerBuilder, IContainer> essentialFrameBuilder, Assembly[] assemblies)
    {
        ContainerBuilder containerBuilder = essentialFrameBuilder.Builder;

        containerBuilder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(IAggregateRepository<,>))
            .InstancePerLifetimeScope();

        containerBuilder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(ISnapshotRepository<,>))
            .InstancePerLifetimeScope();

        containerBuilder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(ISnapshotStrategy<,>))
            .InstancePerLifetimeScope();
        
        return essentialFrameBuilder;
    }
}
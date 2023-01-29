using Autofac;
using EssentialFrame.Cache.Interfaces;

namespace EssentialFrame.Cache.Di;

public static class CacheRegistration
{
    public static IEssentialFrameBuilder<ContainerBuilder, IContainer> AddMemoryCache(
        this IEssentialFrameBuilder<ContainerBuilder, IContainer> essentialFrameBuilder)
    {
        ContainerBuilder containerBuilder = essentialFrameBuilder.Builder;

        containerBuilder.RegisterGeneric(typeof(GuidCache<>)).As(typeof(ICache<,>)).SingleInstance();
        containerBuilder.RegisterGeneric(typeof(StringCache<>)).As(typeof(ICache<,>)).SingleInstance();
        containerBuilder.RegisterGeneric(typeof(IntCache<>)).As(typeof(ICache<,>)).SingleInstance();

        return essentialFrameBuilder;
    }
}
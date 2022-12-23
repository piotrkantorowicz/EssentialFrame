using Autofac;

namespace EssentialFrame.Cache.MemoryCache.Autofac;

internal static class AutofacMemoryCacheRegistration
{
    public static IEssentialFrameBuilder<ContainerBuilder, IContainer> AddMemoryCache(
        this IEssentialFrameBuilder<ContainerBuilder, IContainer> essentialFrameBuilder)
    {
        var containerBuilder = essentialFrameBuilder.Builder;

        containerBuilder.RegisterGeneric(typeof(GuidCache<>)).As(typeof(ICache<,>)).SingleInstance();

        return essentialFrameBuilder;
    }
}

using System;
using Autofac;

namespace EssentialFrame;

public sealed class DefaultEssentialFrameBuilder : IEssentialFrameBuilder<ContainerBuilder, IContainer>
{
    private IContainer _container;

    public DefaultEssentialFrameBuilder()
    {
        Builder = new ContainerBuilder();
    }

    public DefaultEssentialFrameBuilder(ContainerBuilder containerBuilder)
    {
        Builder = containerBuilder ?? throw new ArgumentNullException(nameof(containerBuilder));
    }

    public ContainerBuilder Builder { get; }

    public bool IsBuilt()
    {
        return _container is not null;
    }

    public IContainer Build()
    {
        _container = Builder.Build();

        return _container;
    }
}
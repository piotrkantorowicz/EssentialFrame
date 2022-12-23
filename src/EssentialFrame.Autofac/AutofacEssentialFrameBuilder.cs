using Autofac;

namespace EssentialFrame.Autofac;

public sealed class AutofacEssentialFrameBuilder : IEssentialFrameBuilder<ContainerBuilder, IContainer>
{
    private IContainer _container;

    public AutofacEssentialFrameBuilder() => Builder = new ContainerBuilder();

    public AutofacEssentialFrameBuilder(ContainerBuilder containerBuilder) =>
        Builder = containerBuilder ?? throw new ArgumentNullException(nameof(containerBuilder));

    public bool IsBuilt() => _container is not null;

    public ContainerBuilder Builder { get; }

    public IContainer Build()
    {
        _container = Builder.Build();

        return _container;
    }
}



namespace EssentialFrame;

public interface IEssentialFrameBuilder<out TBuilder, out TContainer> where TBuilder : class where TContainer : class
{
    TBuilder Builder { get; }

    bool IsBuilt();

    TContainer Build();
}
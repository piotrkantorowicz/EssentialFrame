using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Domain.Posts.Repositories;

public interface IPostRepository 
{
    Post Get(PostIdentifier postIdentifier);

    Task<Post> GetAsync(PostIdentifier postIdentifier, CancellationToken cancellationToken);

    void Save(Post post);

    Task SaveAsync(Post post, CancellationToken cancellationToken);
    
    void Box(PostIdentifier postIdentifier);

    Task BoxAsync(PostIdentifier postIdentifier, CancellationToken cancellationToken);

    Post Unbox(PostIdentifier postIdentifier);

    Task<Post> UnboxAsync(PostIdentifier postIdentifier, CancellationToken cancellationToken);
}
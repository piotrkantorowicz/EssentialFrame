using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Domain.Posts.Repositories;

public interface IPostRepository 
{
    Post Get(PostIdentifier postIdentifier);

    Task<Post> GetAsync(PostIdentifier postIdentifier, CancellationToken cancellationToken = default);

    void Save(Post post);

    Task SaveAsync(Post post, CancellationToken cancellationToken = default);

    void Box(PostIdentifier postIdentifier);

    Task BoxAsync(PostIdentifier postIdentifier, CancellationToken cancellationToken = default);
}
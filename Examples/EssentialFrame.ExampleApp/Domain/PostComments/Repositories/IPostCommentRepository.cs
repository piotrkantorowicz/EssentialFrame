using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.ExampleApp.Domain.PostComments.Aggregates;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Domain.PostComments.Repositories;

public interface IPostCommentRepository
{
    PostComment Get(PostCommentIdentifier postCommentIdentifier);

    Task<PostComment> GetAsync(PostCommentIdentifier postCommentIdentifier,
        CancellationToken cancellationToken = default);

    void Save(PostComment postComment);

    Task SaveAsync(PostComment postComment, CancellationToken cancellationToken = default);
}
using EssentialFrame.Domain.Persistence.Services.Interfaces;
using EssentialFrame.ExampleApp.Domain.PostComments.Aggregates;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Domain.PostComments.Repositories;

public interface IPostCommentRepository : IAggregateRepository<PostComment, PostCommentIdentifier>
{
}
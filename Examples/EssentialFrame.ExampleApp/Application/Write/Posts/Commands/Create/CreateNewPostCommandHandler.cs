using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.Create;

internal sealed class CreateNewPostCommandHandler : ICommandHandler<CreateNewPostCommand>
{
    public ICommandResult Handle(CreateNewPostCommand command)
    {
        throw new NotImplementedException();
    }
}

internal sealed class AsyncCreateNewPostCommandHandler : IAsyncCommandHandler<CreateNewPostCommand>
{
    public Task<ICommandResult> HandleAsync(CreateNewPostCommand command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
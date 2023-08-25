using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.ChangeTitle;

internal sealed class ChangeTitleCommandHandler : ICommandHandler<ChangeTitleCommand>
{
    public ICommandResult Handle(ChangeTitleCommand command)
    {
        throw new NotImplementedException();
    }
}

internal sealed class AsyncChangeTitleCommandHandler : IAsyncCommandHandler<ChangeTitleCommand>
{
    public Task<ICommandResult> HandleAsync(ChangeTitleCommand command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
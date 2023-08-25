using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.ChangeDescription;

internal sealed class ChangeDescriptionCommandHandler : ICommandHandler<ChangeDescriptionCommand>
{
    public ICommandResult Handle(ChangeDescriptionCommand command)
    {
        throw new NotImplementedException();
    }
}

internal sealed class AsyncChangeDescriptionCommandHandler : IAsyncCommandHandler<ChangeDescriptionCommand>
{
    public Task<ICommandResult> HandleAsync(ChangeDescriptionCommand command,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
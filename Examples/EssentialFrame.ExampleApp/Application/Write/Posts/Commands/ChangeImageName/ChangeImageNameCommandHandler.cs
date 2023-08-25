using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.ChangeImageName;

internal sealed class ChangeImageNameCommandHandler : ICommandHandler<ChangeImageNameCommand>
{
    public ICommandResult Handle(ChangeImageNameCommand command)
    {
        throw new NotImplementedException();
    }
}

internal sealed class AsyncChangeImageNameCommandHandler : IAsyncCommandHandler<ChangeImageNameCommand>
{
    public Task<ICommandResult> HandleAsync(ChangeImageNameCommand command,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.ExtendedExpirationDate;

internal sealed class ExtendedExpirationDateCommandHandler : ICommandHandler<ExtendedExpirationDateCommand>
{
    public ICommandResult Handle(ExtendedExpirationDateCommand command)
    {
        throw new NotImplementedException();
    }
}

internal sealed class AsyncExtendedExpirationDateCommandHandler : IAsyncCommandHandler<ExtendedExpirationDateCommand>
{
    public Task<ICommandResult> HandleAsync(ExtendedExpirationDateCommand command,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
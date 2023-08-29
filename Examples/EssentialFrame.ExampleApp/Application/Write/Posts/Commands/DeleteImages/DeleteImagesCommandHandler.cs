using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.DeleteImages;

internal sealed class DeleteImagesCommandHandler : ICommandHandler<DeleteImagesCommand>
{
    public ICommandResult Handle(DeleteImagesCommand command)
    {
        throw new NotImplementedException();
    }
}

internal sealed class AsyncDeleteImagesCommandHandler : IAsyncCommandHandler<DeleteImagesCommand>
{
    public Task<ICommandResult> HandleAsync(DeleteImagesCommand command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
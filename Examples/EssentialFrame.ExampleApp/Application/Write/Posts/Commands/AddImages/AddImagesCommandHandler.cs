using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.AddImages;

internal sealed class AddImagesCommandHandler : ICommandHandler<AddImagesCommand>
{
    public ICommandResult Handle(AddImagesCommand command)
    {
        throw new NotImplementedException();
    }
}

internal sealed class AsyncAddImagesCommandHandler : IAsyncCommandHandler<AddImagesCommand>
{
    public Task<ICommandResult> HandleAsync(AddImagesCommand command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
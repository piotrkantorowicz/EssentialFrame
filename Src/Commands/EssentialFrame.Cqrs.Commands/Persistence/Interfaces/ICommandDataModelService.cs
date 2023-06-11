using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Persistence.Models;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Cqrs.Commands.Persistence.Interfaces;

public interface ICommandDataModelService
{
    CommandDataModel Create(ICommand command);

    CommandDataModel Create(ICommand command, ISerializer serializer);

    void Start(CommandDataModel commandDataModel);

    void Schedule(CommandDataModel commandDataModel, DateTimeOffset? at);

    void Cancel(CommandDataModel commandDataModel);

    void Complete(CommandDataModel commandDataModel, bool success);
}
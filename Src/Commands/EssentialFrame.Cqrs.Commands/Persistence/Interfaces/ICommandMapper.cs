using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Persistence.Models;

namespace EssentialFrame.Cqrs.Commands.Persistence.Interfaces;

public interface ICommandMapper
{
    CommandDataModel Map(ICommand command);

    IReadOnlyCollection<CommandDataModel> Map(IEnumerable<ICommand> commands);

    ICommand Map(CommandDataModel commandDataModel);

    IReadOnlyCollection<ICommand> Map(IEnumerable<CommandDataModel> commandDataModels);
}
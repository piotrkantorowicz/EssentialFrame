using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Persistence.Models;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Cqrs.Commands.Persistence.Interfaces;

public interface ICommandMapper
{
    CommandDataModel Map(ICommand command);

    CommandDataModel Map(ICommand command, ISerializer serializer);

    IReadOnlyCollection<CommandDataModel> Map(IEnumerable<ICommand> commands);

    IReadOnlyCollection<CommandDataModel> Map(IEnumerable<ICommand> commands, ISerializer serializer);

    ICommand Map(CommandDataModel commandDataModel);

    ICommand Map(CommandDataModel commandDataModel, ISerializer serializer);

    IReadOnlyCollection<ICommand> Map(IEnumerable<CommandDataModel> commandDataModels);

    IReadOnlyCollection<ICommand> Map(IEnumerable<CommandDataModel> commandDataModels, ISerializer serializer);
}
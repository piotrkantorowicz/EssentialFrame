using EssentialFrame.Cqrs.Commands.Store.Interfaces;
using EssentialFrame.Serialization.Interfaces;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Cqrs.Commands.UnitTests.Store;

[TestFixture]
public class CommandsRepositoryTests
{
    private readonly Mock<ICommandStore> _commandStoreMock;
    private readonly Mock<ISerializer> _serializerMock;

    [Test]
    public void StartExecution_WhenCommandIsProvided_ShouldSaveCommandToStore()
    {
        // Arrange
    }
}
using Autofac;
using EssentialFrame.Cqrs.Commands.Interfaces;
using EssentialFrame.Cqrs.Commands.Logging.Decorators;

namespace EssentialFrame.Cqrs.Commands.Logging.Autofac;

public static class AutofacRegistration
{
    public static ContainerBuilder AddCqrsCommandsLogging(this ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterGenericDecorator(typeof(LoggingAsyncCommandHandlerDecorator<>),
                                                  typeof(IAsyncCommandHandler<>));

        containerBuilder.RegisterGenericDecorator(typeof(LoggingCommandHandlerDecorator<>),
                                                  typeof(ICommandHandler<>));

        return containerBuilder;
    }
}


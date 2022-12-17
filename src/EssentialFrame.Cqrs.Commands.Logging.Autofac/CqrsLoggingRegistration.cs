using System.Reflection;
using Autofac;
using EssentialFrame.Cqrs.Commands.Interfaces;
using EssentialFrame.Cqrs.Commands.Logging.Decorators;

namespace EssentialFrame.Cqrs.Commands.Logging.Autofac;

public static class CqrsLoggingRegistration
{
    public static ContainerBuilder AddCqrsCommandsLogging(this ContainerBuilder containerBuilder,
                                                          Assembly[] assemblies)
    {
        containerBuilder.RegisterGenericDecorator(typeof(LoggingAsyncCommandHandlerDecorator<>),
                                                  typeof(IAsyncCommandHandler<>));

        containerBuilder.RegisterGenericDecorator(typeof(LoggingCommandHandlerDecorator<>),
                                                  typeof(ICommandHandler<>));

        return containerBuilder;
    }
}


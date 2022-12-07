using System.Reflection;
using Autofac;
using EssentialFrame.Cqrs.Interfaces;
using EssentialFrame.Cqrs.Logging.Decorators;

namespace EssentialFrame.Cqrs.Logging.Autofac;

public static class CqrsLoggingRegistration
{
    public static ContainerBuilder AddCqrsLogging(this ContainerBuilder containerBuilder,
                                                  Assembly[] assemblies)
    {
        containerBuilder.RegisterGenericDecorator(typeof(LoggingCommandDecorator<>),
                                                  typeof(ICommandHandler<>));

        containerBuilder.RegisterGenericDecorator(typeof(LoggingQueryHandlerDecorator<,>),
                                                  typeof(IQueryHandler<,>));

        return containerBuilder;
    }
}

using Autofac;
using EssentialFrame.Cqrs.Queries.Interfaces;
using EssentialFrame.Cqrs.Queries.Logging.Decorators;

namespace EssentialFrame.Cqrs.Queries.Logging.Autofac;

public static class AutofacRegistration
{
    public static ContainerBuilder AddCqrsQueriesLogging(this ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterGenericDecorator(typeof(LoggingAsyncQueryHandlerDecorator<,>),
                                                  typeof(IAsyncQueryHandler<,>));

        containerBuilder.RegisterGenericDecorator(typeof(LoggingQueryHandlerDecorator<,>),
                                                  typeof(IQueryHandler<,>));

        return containerBuilder;
    }
}


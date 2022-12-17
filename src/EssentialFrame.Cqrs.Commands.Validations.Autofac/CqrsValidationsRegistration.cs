using System.Reflection;
using Autofac;
using EssentialFrame.Cqrs.Commands.Interfaces;
using EssentialFrame.Cqrs.Commands.Validations.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Validations.Decorators;

namespace EssentialFrame.Cqrs.Commands.Validations.Autofac;

public static class CqrsValidationsRegistration
{
    public static ContainerBuilder AddCqrsValidation(this ContainerBuilder containerBuilder,
                                                     Assembly[] assemblies)
    {
        containerBuilder.RegisterAssemblyTypes(assemblies)
                        .AsClosedTypesOf(typeof(ICommandValidator<>))
                        .InstancePerLifetimeScope();

        containerBuilder.RegisterGenericDecorator(typeof(ValidationAsyncCommandHandlerDecorator<>),
                                                  typeof(IAsyncCommandHandler<>));

        containerBuilder.RegisterGenericDecorator(typeof(ValidationCommandHandlerDecorator<>),
                                                  typeof(ICommandHandler<>));

        return containerBuilder;
    }
}


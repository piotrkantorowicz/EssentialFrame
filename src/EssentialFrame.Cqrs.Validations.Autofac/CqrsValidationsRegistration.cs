using System.Reflection;
using Autofac;
using EssentialFrame.Cqrs.Interfaces;
using EssentialFrame.Cqrs.Validations.Core.Interfaces;
using EssentialFrame.Cqrs.Validations.Decorators;

namespace EssentialFrame.Cqrs.Validations.Autofac;

public static class CqrsValidationsRegistration
{
    public static ContainerBuilder AddCqrsValidation(this ContainerBuilder containerBuilder,
                                                     Assembly[] assemblies)
    {
        containerBuilder.RegisterAssemblyTypes(assemblies)
                        .AsClosedTypesOf(typeof(ICommandValidator<>))
                        .InstancePerLifetimeScope();

        containerBuilder.RegisterGenericDecorator(typeof(ValidationCommandHandlerDecorator<>),
                                                  typeof(ICommandHandler<>));

        return containerBuilder;
    }
}

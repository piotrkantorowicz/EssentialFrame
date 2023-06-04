using System.Reflection;
using Autofac;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Validations.Decorators;
using FluentValidation;

namespace EssentialFrame.Cqrs.Commands.Validations.Di;

public static class CommandsValidationsRegistration
{
    public static IEssentialFrameBuilder<ContainerBuilder, IContainer> AddCommandsValidation(
        this IEssentialFrameBuilder<ContainerBuilder, IContainer> essentialFrameBuilder, Assembly[] assemblies)
    {
        ContainerBuilder containerBuilder = essentialFrameBuilder.Builder;

        containerBuilder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(IValidator<>))
            .InstancePerLifetimeScope();

        containerBuilder.RegisterGenericDecorator(typeof(ValidationAsyncCommandHandlerDecorator<>),
            typeof(IAsyncCommandHandler<>));

        containerBuilder.RegisterGenericDecorator(typeof(ValidationCommandHandlerDecorator<>),
            typeof(ICommandHandler<>));

        return essentialFrameBuilder;
    }
}
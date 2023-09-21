using System;

namespace EssentialFrame.Identity.Interfaces;

public interface IService
{
    public Guid Identifier { get; }

    public string Name { get; }

    public string Version { get; }

    public string GetFullIdentifier();
}
using System;

namespace EssentialFrame.Identity;

public interface IService
{
    public Guid Identifier { get; }

    public string Name { get; }

    public string GetFullIdentifier();
}
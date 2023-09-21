using System;
using EssentialFrame.Identity.Interfaces;

namespace EssentialFrame.Identity;

public class Service : IService
{
    public Service(Guid identifier, string name, string version)
    {
        Identifier = identifier;
        Name = name;
        Version = version;
    }

    public virtual Guid Identifier { get; }

    public virtual string Name { get; }

    public virtual string Version { get; }

    public string GetFullIdentifier()
    {
        return $"{Name}-{Version}-{Identifier}";
    }
}
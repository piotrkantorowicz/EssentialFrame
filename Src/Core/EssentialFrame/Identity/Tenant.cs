using System;
using EssentialFrame.Identity.Interfaces;

namespace EssentialFrame.Identity;

public class Tenant : ITenant
{
    public Tenant(Guid identifier, string code, string name, int key)
    {
        Identifier = identifier;
        Code = code;
        Name = name;
        Key = key;
    }

    public virtual Guid Identifier { get; }

    public virtual string Code { get; }

    public virtual string Name { get; }

    public virtual int Key { get; }
}
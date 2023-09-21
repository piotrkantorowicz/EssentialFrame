using System;
using EssentialFrame.Identity.Interfaces;

namespace EssentialFrame.Identity;

public class User : IUser
{
    public User(Guid identifier, string email, string name, bool isAuthenticated, int key)
    {
        Identifier = identifier;
        Email = email;
        Name = name;
        IsAuthenticated = isAuthenticated;
        Key = key;
    }

    public virtual Guid Identifier { get; }

    public virtual string Email { get; }

    public virtual string Name { get; }

    public virtual bool IsAuthenticated { get; }

    public virtual int Key { get; }
}
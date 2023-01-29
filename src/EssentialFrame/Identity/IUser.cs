using System;

namespace EssentialFrame.Identity;

public interface IUser
{
    Guid Identifier { get; }

    string Email { get; }

    string Name { get; }

    bool IsAuthenticated { get; }

    int Key { get; }
}
using System;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.Posts.Identity;

public class User : IUser
{
    public User()
    {
        Identifier = new Guid("4D0AB6DC-84CA-48A9-8FEB-3BF93313D01D");
        Email = "test.user@email.com";
        Name = "Test user";
        IsAuthenticated = true;
        Key = 1234567890;
    }

    public Guid Identifier { get; }

    public string Email { get; }

    public string Name { get; }

    public bool IsAuthenticated { get; }

    public int Key { get; }
}
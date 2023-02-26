using System;
using EssentialFrame.Identity;

namespace EssentialFrame.TestData.Identity;

public sealed class TestService : IService
{
    public TestService()
    {
        Identifier = new Guid("6F2D5BFE-622D-460D-BC60-D1290098498A");
        Name = "Test Service";
        Version = "1.0.0";
    }

    public Guid Identifier { get; }

    public string Name { get; }

    public string Version { get; }

    public string GetFullIdentifier()
    {
        return $"{Name}-{Version}-{Identifier}";
    }
}
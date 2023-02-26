using System;
using EssentialFrame.Identity;

namespace EssentialFrame.TestData.Identity;

public class TestTenant : ITenant
{
    public TestTenant()
    {
        Identifier = new Guid("73EF7E95-6718-440B-8EBC-2B1396FAAAAF");
        Code = "EF";
        Name = "EssentialFrame";
        Key = 1234567890;
    }

    public Guid Identifier { get; }

    public string Code { get; }

    public string Name { get; }

    public int Key { get; }
}
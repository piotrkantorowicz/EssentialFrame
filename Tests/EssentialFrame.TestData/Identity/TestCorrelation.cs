using System;
using EssentialFrame.Identity;

namespace EssentialFrame.TestData.Identity;

public class TestCorrelation : ICorrelation
{
    public TestCorrelation()
    {
        Identifier = new Guid("9534092F-8FC5-4488-9DFD-AEC47E19387C");
    }

    public Guid Identifier { get; }
}
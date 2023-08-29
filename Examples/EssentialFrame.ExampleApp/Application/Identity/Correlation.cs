using System;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Application.Identity;

public class Correlation : ICorrelation
{
    public Correlation()
    {
        Identifier = new Guid("9534092F-8FC5-4488-9DFD-AEC47E19387C");
    }

    public Guid Identifier { get; }
}
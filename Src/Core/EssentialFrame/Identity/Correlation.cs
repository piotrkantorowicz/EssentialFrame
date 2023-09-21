using System;
using EssentialFrame.Identity.Interfaces;

namespace EssentialFrame.Identity;

public class Correlation : ICorrelation
{
    public Correlation(Guid identifier)
    {
        Identifier = identifier;
    }

    public virtual Guid Identifier { get; }
}
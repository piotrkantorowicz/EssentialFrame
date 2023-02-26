using System;

namespace EssentialFrame.Identity;

public interface ICorrelation
{
    Guid Identifier { get; }
}
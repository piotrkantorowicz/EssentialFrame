using System;

namespace EssentialFrame.Identity.Interfaces;

public interface ICorrelation
{
    Guid Identifier { get; }
}
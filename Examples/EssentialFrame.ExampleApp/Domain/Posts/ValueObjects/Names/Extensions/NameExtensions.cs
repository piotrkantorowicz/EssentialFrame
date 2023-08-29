using System;
using System.Collections.Generic;
using System.Linq;

namespace EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names.Extensions;

public static class NameExtensions
{
    public static bool Contains(this IEnumerable<Name> names, Name name, StringComparer stringComparer = null)
    {
        IEnumerable<string> values = names.Select(x => x.Value);

        return values.Contains(name.Value, stringComparer ?? StringComparer.InvariantCultureIgnoreCase);
    }
}
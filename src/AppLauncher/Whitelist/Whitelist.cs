using System.Collections.Generic;
using System.Linq;

namespace AppLauncher.Whitelist;

public record Whitelist(bool IsConfigured, IEnumerable<string> Entries)
{
    public Whitelist(IEnumerable<string> Entries)
        : this(true, Entries)
    {
    }

    public static Whitelist NotConfigured => new(false, Enumerable.Empty<string>());
}
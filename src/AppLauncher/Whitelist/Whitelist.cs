using System.Collections.Generic;

namespace AppLauncher.Whitelist;

public record Whitelist(IEnumerable<string> Entries);
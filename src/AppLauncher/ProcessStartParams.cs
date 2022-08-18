using System.Diagnostics;

namespace AppLauncher;

internal record ProcessStartParams(string PathToApplication, bool CreateNoWindow = true, ProcessWindowStyle ProcessWindowStyle = ProcessWindowStyle.Normal);
using System;
using System.Diagnostics;

namespace AppLauncher;

public static class WindowsEventLog
{
    private const int EventId = 99;
    private static string CurrentAppName { get; } = AppDomain.CurrentDomain.FriendlyName;

    public static void LogEvent(string message)
    {
        Log(EventLogEntryType.Information, message);
    }

    public static void LogError(string message)
    {
        Log(EventLogEntryType.Error, message);
    }

    private static void Log(EventLogEntryType eventLogEntryType, string message)
    {
        using var eventLog = new EventLog("Application");
        eventLog.Source = CurrentAppName;
        eventLog.WriteEntry(message, eventLogEntryType, EventId);
    }

    public static void CreateEventSource()
    {
        CreateEventSource(CurrentAppName);
    }

    private static void CreateEventSource(string currentAppName)
    {
        // searching the source throws a security exception ONLY if not exists!
        var sourceExists = EventLog.SourceExists(currentAppName);
        if (sourceExists)
        {
            LogEvent($"EventSource {currentAppName} exists.");
            return;
        }

        // no exception until yet means the user as admin privilege
        EventLog.CreateEventSource(currentAppName, "Application");
        LogEvent($"EventSource {currentAppName} created.");
    }
}
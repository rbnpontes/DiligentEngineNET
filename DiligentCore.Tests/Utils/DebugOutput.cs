using System.Text;

namespace Diligent.Tests.Utils;

public static class DebugOutput
{
    public static void MessageCallback(DebugMessageSeverity severity, string message, string function, string file, int line)
    {
        var log = new StringBuilder();
        log.Append($"[{severity}]: ");
        log.Append(message);
        log.Append($" | Function: {function} | File: {file} | Line: {line}");
        var output = log.ToString();
        Console.WriteLine(output);
        System.Diagnostics.Debug.WriteLine(output);
    }
}
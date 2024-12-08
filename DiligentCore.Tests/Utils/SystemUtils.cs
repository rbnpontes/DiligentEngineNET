namespace Diligent.Tests.Utils;

public static class SystemUtils
{
    public static bool IsRunningAsHeadless()
    {
        if (!OperatingSystem.IsLinux())
            return false;
        return string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DISPLAY"));
    }
}
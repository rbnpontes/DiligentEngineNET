namespace Diligent.Tests.Utils;

public static class SystemUtils
{
    public static bool IsRunningAsHeadless()
    {
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI")))
            return true;
        if (!OperatingSystem.IsLinux())
            return false;
        return string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DISPLAY"));
    }
}
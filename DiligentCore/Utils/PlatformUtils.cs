namespace Diligent.Utils;

internal static class PlatformUtils
{
    public static readonly bool IsWasm = OperatingSystem.IsWasi() || OperatingSystem.IsBrowser();
}
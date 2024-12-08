using System.Reflection;
using System.Runtime.InteropServices;

namespace Diligent.Utils;

public static class DiligentLibraryResolver
{
    public static IntPtr Resolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        var runtimesPath = Path.Join(Environment.CurrentDirectory, "runtimes");
        if (OperatingSystem.IsWindows())
        {
            runtimesPath = Path.Join(runtimesPath, "win-x64");
            libraryName += ".dll";
        }
        else if (OperatingSystem.IsLinux())
        {
            runtimesPath = Path.Join(runtimesPath, "linux-x64");
            libraryName = "lib" + libraryName + ".so";
        }

        runtimesPath = Path.Join(runtimesPath, "native", libraryName);
        return NativeLibrary.Load(runtimesPath);
    }
}
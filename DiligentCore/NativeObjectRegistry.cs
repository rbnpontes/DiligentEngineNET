using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Diligent;

internal static class NativeObjectRegistry
{
    private static Dictionary<IntPtr, WeakReference<INativeObject>> Registry = new();
    public static void AddToRegister(IntPtr nativePointer, INativeObject obj)
    {
        Registry.TryAdd(nativePointer, new WeakReference<INativeObject>(obj));
    }

    public static void RemoveObject(IntPtr handle)
    {
        Registry.Remove(handle);
    }
    
    public static bool TryGetObject(IntPtr nativePointer, out INativeObject? output)
    {
        if (!Registry.TryGetValue(nativePointer, out var obj))
        {
            output = null;
            return false;
        }

        return obj.TryGetTarget(out output);
    }
}
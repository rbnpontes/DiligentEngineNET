using System.Runtime.CompilerServices;

namespace Diligent;

internal static class NativeObjectRegistry
{
    private static Dictionary<IntPtr, WeakReference<INativeObject>> Registry = new();

    public static void AddToRegister(INativeObject obj)
    {
        Registry.Add(obj.Handle, new WeakReference<INativeObject>(obj));
    }

    public static bool TryGetObject(IntPtr nativePointer, out INativeObject? output)
    {
        if (!Registry.TryGetValue(nativePointer, out var obj))
        {
            output = null;
            return false;
        }

        if (obj.TryGetTarget(out output))
            return true;

        Registry.Remove(nativePointer);
        return false;
    }
}
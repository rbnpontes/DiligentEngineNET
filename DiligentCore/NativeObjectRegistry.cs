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

    public static T GetOrCreate<T>(Func<T> creationCall, IntPtr handle) where T : INativeObject
    {
        if (!TryGetObject(handle, out var output))
        {
            var result = creationCall();
            AddToRegister(handle, result);
            return result;
        }

        var target = (T?)output;
        return target ?? throw new InvalidCastException($"Failed cast to type '{typeof(T).Name}'");
    }
}
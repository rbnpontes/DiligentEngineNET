using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Diligent;

internal static class NativeObjectRegistry
{
    private static readonly Dictionary<IntPtr, WeakReference<INativeObject>> sRegistry = new();
    public static void AddToRegister(IntPtr nativePointer, INativeObject obj)
    {
        sRegistry.TryAdd(nativePointer, new WeakReference<INativeObject>(obj));
    }

    public static void RemoveObject(IntPtr handle)
    {
        sRegistry.Remove(handle);
    }
    
    public static bool TryGetObject(IntPtr nativePointer, out INativeObject? output)
    {
        if (sRegistry.TryGetValue(nativePointer, out var obj)) 
            return obj.TryGetTarget(out output);
        output = null;
        return false;

    }

    public static bool TryGetObject<T>(IntPtr nativePointer, out T? output) where T : INativeObject
    {
        var result = TryGetObject(nativePointer, out INativeObject? resultObj);
        output = (T?)resultObj;
        return result;
    }

    public static T GetOrCreate<T>(Func<T> creationCall, IntPtr handle) where T : INativeObject
    {
        if (TryGetObject(handle, out var output))
        {
            var target = (T?)output;
            return target ?? throw new InvalidCastException($"Failed cast to type '{typeof(T).Name}'");
        }

        var result = creationCall();
        AddToRegister(handle, result);
        return result;
    }
}
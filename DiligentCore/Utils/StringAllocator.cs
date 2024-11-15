using System.Runtime.InteropServices;

namespace Diligent.Utils;

internal class StringAllocator : IDisposable
{
    private Queue<IntPtr> _allocatedPointers = new();

    public IntPtr Acquire(string input)
    {
        if (string.IsNullOrEmpty(input))
            return IntPtr.Zero;
        var result = Marshal.StringToHGlobalAnsi(input);
        _allocatedPointers.Enqueue(result);
        return result;
    }

    public void Dispose()
    {
        while(_allocatedPointers.TryDequeue(out var ptr))
            Marshal.FreeHGlobal(ptr);
    }
}
using System.Runtime.InteropServices;

namespace Diligent.Utils;

internal class StringAllocator : IDisposable
{
    private interface IAllocator : IDisposable
    {
        public IntPtr Alloc(string input);
    }

    private class DefaultAllocator : IAllocator
    {
        private readonly Queue<IntPtr> _allocatedPointers = new();

        public IntPtr Alloc(string input)
        {
            if (string.IsNullOrEmpty(input))
                return IntPtr.Zero;
            var result = Marshal.StringToHGlobalAnsi(input);
            _allocatedPointers.Enqueue(result);
            return result;
        }

        public void Dispose()
        {
            while (_allocatedPointers.TryDequeue(out var ptr))
                Marshal.FreeHGlobal(ptr);
        }
    }

    private class WebAllocator : IAllocator
    {
        private readonly Queue<IntPtr> _allocatedPointers = new();

        public IntPtr Alloc(string input)
        {
            if (string.IsNullOrEmpty(input))
                return IntPtr.Zero;
            var result = WebInteropUtils.AllocString(input);
            _allocatedPointers.Enqueue(result);
            return result;
        }

        public void Dispose()
        {
            while (_allocatedPointers.TryDequeue(out var ptr))
                WebInteropUtils.MemoryFree(ptr);
        }
    }
    
    private readonly IAllocator _allocator;

    public StringAllocator()
    {
        if (PlatformUtils.IsWasm)
            _allocator = new WebAllocator();
        else
            _allocator = new DefaultAllocator();
    }
    
    public IntPtr Acquire(string input) => _allocator.Alloc(input);

    public void Dispose() => _allocator.Dispose();
}
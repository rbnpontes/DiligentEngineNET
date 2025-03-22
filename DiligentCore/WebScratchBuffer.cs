namespace Diligent;

internal static class WebScratchBuffer
{
    private static IntPtr _currMemory = IntPtr.Zero;
    private static uint _currSize = 0;

    private static void AssertPlatform()
    {
        var isWasm = OperatingSystem.IsWasi() || OperatingSystem.IsBrowser();
        if(!isWasm)
            throw new PlatformNotSupportedException("Web Assembly call only");
    }

    private static void ResetMemory(int size)
    {
        WebInteropUtils.MemorySet(_currMemory, 0, size);
    }

    public static IntPtr Require(int memorySize)
    {
        return Require((uint)memorySize);
    }
    public static IntPtr Require(uint memorySize)
    {
        AssertPlatform();
        if (_currMemory == IntPtr.Zero)
        {
            _currMemory = WebInteropUtils.MemoryAlloc((int)memorySize);
            _currSize = memorySize;
            ResetMemory((int)memorySize);
            return _currMemory;
        }

        if (memorySize <= _currSize) 
            return _currMemory;
        
        Free();
        _currSize = memorySize;
        _currMemory = WebInteropUtils.MemoryAlloc((int)memorySize);
        ResetMemory((int)memorySize);
        return _currMemory;
    }

    public static void Free()
    {
        AssertPlatform();
        WebInteropUtils.MemoryFree(_currMemory);
        _currMemory = IntPtr.Zero;
        _currSize = 0;
    }
}
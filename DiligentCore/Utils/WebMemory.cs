using Diligent;

namespace Diligent.Utils;

internal class WebMemory : IDisposable
{
    public IntPtr Handle { get; private set; }
    
    public WebMemory(uint size)
    {
        var isWasm = OperatingSystem.IsBrowser() || OperatingSystem.IsWasi();
        if(!isWasm)
            throw new PlatformNotSupportedException("WebMemory is supported on Web Assembly only");
        
        Handle = WebInteropUtils.MemoryAlloc((int)size);
        WebInteropUtils.MemorySet(Handle, 0x0, (int)size);
    }

    public WebMemory(int size) : this((uint)size){}
    
    public void Dispose()
    {
        WebInteropUtils.MemoryFree(Handle);
    }
}
using System.Runtime.InteropServices;

namespace Diligent;

internal class BrowserWindowHandle : WindowHandle
{
    private readonly IntPtr _canvasIdPtr;

    public BrowserWindowHandle(string canvasId)
    {
        _canvasIdPtr = WebInteropUtils.AllocString(canvasId);
        UpdateInternalWindowHandle(_canvasIdPtr);
    }
    
    ~BrowserWindowHandle()
    {
        WebInteropUtils.MemoryFree(_canvasIdPtr);
    }
}
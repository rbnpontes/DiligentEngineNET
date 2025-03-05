using System.Runtime.InteropServices;

namespace Diligent;

internal class BrowserWindowHandle : WindowHandle
{
    private readonly IntPtr _canvasIdPtr;

    public BrowserWindowHandle(string canvasId)
    {
        _canvasIdPtr = Marshal.StringToHGlobalAnsi(canvasId);
        UpdateInternalWindowHandle(_canvasIdPtr);
    }
    
    ~BrowserWindowHandle()
    {
        Marshal.FreeHGlobal(_canvasIdPtr);
    }
}
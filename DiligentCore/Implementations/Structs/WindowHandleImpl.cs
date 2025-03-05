namespace Diligent;

public partial class WindowHandle
{
    internal LinuxWindowHandle LinuxWindowHandle { get; set; } = new();
    // lock constructor to internal
    internal WindowHandle() {}

    internal WindowHandle(IntPtr ptr)
    {
        UpdateInternalWindowHandle(ptr);
    }
    
    protected void UpdateInternalWindowHandle(IntPtr handle)
    {
        _data.window_handle_ = handle;
    }
}
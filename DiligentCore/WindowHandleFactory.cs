namespace Diligent;

public static class WindowHandleFactory
{
    public static WindowHandle CreateWin32Window(IntPtr hwnd)
    {
        if (!OperatingSystem.IsWindows())
            throw new PlatformNotSupportedException();

        return CreateFromWindowPointer(hwnd);
    }

    public static WindowHandle CreateUwpWindow(IntPtr coreWindow)
    {
        var osPlatform = Environment.OSVersion.Platform;
        if (!(OperatingSystem.IsWindows() || osPlatform == PlatformID.Xbox))
            throw new PlatformNotSupportedException();

        return CreateFromWindowPointer(coreWindow);
    }

    public static WindowHandle CreateLinuxWindow(LinuxWindowHandle linuxWindowHandle)
    {
        if (!OperatingSystem.IsLinux())
            throw new PlatformNotSupportedException();

        var wnd = new WindowHandle();
        wnd.LinuxWindowHandle = linuxWindowHandle;
        return wnd;
    }

    public static WindowHandle CreateMacOsWindow(IntPtr nsView)
    {
        if (!OperatingSystem.IsMacOS())
            throw new PlatformNotSupportedException();

        return CreateFromWindowPointer(nsView);
    }

    public static WindowHandle CreateTvOsWindow(IntPtr caLayer)
    {
        if (!OperatingSystem.IsTvOS())
            throw new PlatformNotSupportedException();

        return CreateFromWindowPointer(caLayer);
    }

    public static WindowHandle CreateIosWindow(IntPtr caLayer)
    {
        if (!OperatingSystem.IsIOS())
            throw new PlatformNotSupportedException();

        return CreateIosWindow(caLayer);
    }

    public static WindowHandle CreateAndroidWindow(IntPtr androidWindow)
    {
        if (!OperatingSystem.IsAndroid())
            throw new PlatformNotSupportedException();

        return CreateFromWindowPointer(androidWindow);
    }

    public static WindowHandle CreateBrowserWindow(string canvasId)
    {
        if (!OperatingSystem.IsBrowser())
            throw new PlatformNotSupportedException();
        return new BrowserWindowHandle(canvasId);
    }

    public static WindowHandle CreateFromWindowPointer(IntPtr systemWindowPtr)
    {
        return new WindowHandle(systemWindowPtr);
    }

    public static WindowHandle CreateNull()
    {
        return new WindowHandle();
    }
}
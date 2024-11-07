namespace Diligent.Utils;

internal static class DiligentObjectsFactory
{
    public static IRenderDevice CreateRenderDevice(IntPtr handle)
    {
        if (handle == IntPtr.Zero)
            throw new NullReferenceException($"Failed to create {nameof(IRenderDevice)}");
        return NativeObjectRegistry.GetOrCreate(() => new RenderDevice(handle), handle);
    }

    public static IDeviceContext[] CreateDeviceContexts(IntPtr[] handle)
    {
        return handle.Select(x =>
        {
            return NativeObjectRegistry.GetOrCreate<IDeviceContext>(() => new DeviceContext(x), x);
        }).ToArray();
    }

    public static ISwapChain CreateSwapChain(IntPtr handle)
    {
        if (handle == IntPtr.Zero)
            throw new NullReferenceException($"Failed to create {nameof(ISwapChain)}");

        return NativeObjectRegistry.GetOrCreate(() => new SwapChain(handle), handle);
    }
}
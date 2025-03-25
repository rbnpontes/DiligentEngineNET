namespace Diligent.Platforms.Default;

internal class SwapChainDefaultImpl(nint handle) : ISwapChainImpl
{
    public unsafe SwapChainDesc GetDesc()
    {
        var swapChainDescPtr = (SwapChainDesc.__Internal*)SwapChain.Interop.swap_chain_get_desc(handle).ToPointer();
        return SwapChainDesc.FromInternalStruct(*swapChainDescPtr);
    }

    public ITextureView GetCurrentBackBufferRTV()
    {
        var ptr = SwapChain.Interop.swap_chain_get_current_back_buffer_rtv(handle);
        return NativeObjectRegistry.GetOrCreate(() => new UnDisposableTextureView(ptr), ptr);
    }

    public ITextureView GetDepthBufferDSV()
    {
        var ptr = SwapChain.Interop.swap_chain_get_depth_buffer_dsv(handle);
        return NativeObjectRegistry.GetOrCreate(() => new UnDisposableTextureView(ptr), ptr);
    }

    public void Present(uint syncInterval)
    {
        SwapChain.Interop.swap_chain_present(handle, syncInterval);
    }

    public void Resize(uint newWidth, uint newHeight, SurfaceTransform transform)
    {
        SwapChain.Interop.swap_chain_resize(handle, newWidth, newHeight, transform);
    }

    public unsafe void SetFullscreenMode(DisplayModeAttribs attribs)
    {
        var displayModeData = DisplayModeAttribs.GetInternalStruct(attribs);
        SwapChain.Interop.swap_chain_set_fullscreen_mode(handle, new IntPtr(&displayModeData));
    }

    public void SetWindowedMode()
    {
        SwapChain.Interop.swap_chain_set_windowed_mode(handle);
    }

    public void SetMaximumFrameLatency(uint value)
    {
        SwapChain.Interop.swap_chain_set_maximum_frame_latency(handle, value);
    }
}
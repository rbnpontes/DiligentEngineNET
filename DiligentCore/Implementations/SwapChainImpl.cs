namespace Diligent;

internal partial class SwapChain : ISwapChain
{
    public unsafe SwapChainDesc Desc
    {
        get
        {
            var swapChainDescPtr = (SwapChainDesc.__Internal*)Interop.swap_chain_get_desc(Handle).ToPointer();
            return SwapChainDesc.FromInternalStruct(*swapChainDescPtr);
        }
    }

    public ITextureView CurrentBackBufferRTV
    {
        get
        {
            var ptr = Interop.swap_chain_get_current_back_buffer_rtv(Handle);
            return NativeObjectRegistry.GetOrCreate(() => new UnDisposableTextureView(ptr), ptr);
        }
    }

    public ITextureView DepthBufferDSV
    {
        get
        {
            var ptr = Interop.swap_chain_get_depth_buffer_dsv(Handle);
            return NativeObjectRegistry.GetOrCreate(() => new UnDisposableTextureView(ptr), ptr);
        }
    }
    
    internal SwapChain(IntPtr handle) : base(handle)
    {
    }

    public void Present(uint syncInterval = 1)
    {
        Interop.swap_chain_present(Handle, syncInterval);
    }

    public void Resize(uint newWidth, uint newHeight, SurfaceTransform newTransform)
    {
        Interop.swap_chain_resize(Handle, newWidth, newHeight, newTransform);
    }

    public unsafe void SetFullscreenMode(DisplayModeAttribs displayMode)
    {
        var displayModeData = DisplayModeAttribs.GetInternalStruct(displayMode);
        Interop.swap_chain_set_fullscreen_mode(Handle, new IntPtr(&displayModeData));
    }

    public void SetWindowedMode()
    {
        Interop.swap_chain_set_windowed_mode(Handle);
    }
    
    public void SetMaximumFrameLatency(uint maxLatency)
    {
        Interop.swap_chain_set_maximum_frame_latency(Handle, maxLatency);
    }
}
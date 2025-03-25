using System.Runtime.CompilerServices;

namespace Diligent.Platforms.Default.Web;

internal class SwapChainWebImpl(nint handle): ISwapChainImpl
{
    public unsafe SwapChainDesc GetDesc()
    {
        var swapChainDescPtr = SwapChain.WebInterop.swap_chain_get_desc(handle);
        var result = nint.Zero;
        
        WebInteropUtils.CopyToDiligent(swapChainDescPtr, &result, (uint)sizeof(nint));
        
        var swapChainDesc = new SwapChainDesc.__Internal();
        WebInteropUtils.CopyToDiligent(result, &swapChainDesc, (uint)sizeof(SwapChainDesc.__Internal));
        return SwapChainDesc.FromInternalStruct(swapChainDesc);
    }

    public ITextureView GetCurrentBackBufferRTV()
    {
        var ptr = SwapChain.WebInterop.swap_chain_get_current_back_buffer_rtv(handle);
        return NativeObjectRegistry.GetOrCreate(() => new UnDisposableTextureView(ptr), ptr);
    }

    public ITextureView GetDepthBufferDSV()
    {
        var ptr = SwapChain.WebInterop.swap_chain_get_depth_buffer_dsv(handle);
        return NativeObjectRegistry.GetOrCreate(() => new UnDisposableTextureView(ptr), ptr);
    }

    public void Present(uint syncInterval)
    {
        SwapChain.WebInterop.swap_chain_present(handle, (int)syncInterval);
    }

    public void Resize(uint newWidth, uint newHeight, SurfaceTransform transform)
    {
        SwapChain.WebInterop.swap_chain_resize(handle, (int)newWidth, (int)newHeight, (int)transform);
    }

    public void SetFullscreenMode(DisplayModeAttribs attribs)
    {
        var data = DisplayModeAttribs.GetInternalStruct(attribs);
        var buffer = WebScratchBuffer.Require(Unsafe.SizeOf<DisplayModeAttribs.__Internal>());
        
        SwapChain.WebInterop.swap_chain_set_fullscreen_mode(handle, buffer);
    }

    public void SetWindowedMode()
    {
        SwapChain.WebInterop.swap_chain_set_windowed_mode(handle);
    }

    public void SetMaximumFrameLatency(uint value)
    {
        SwapChain.WebInterop.swap_chain_set_maximum_frame_latency(handle, (int)value);
    }
}
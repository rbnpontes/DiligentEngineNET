using Diligent.Platforms.Default;
using Diligent.Platforms.Default.Web;
using Diligent.Utils;

namespace Diligent;

internal partial class SwapChain : ISwapChain
{
    private ISwapChainImpl _impl;
    public SwapChainDesc Desc => _impl.GetDesc();

    public ITextureView CurrentBackBufferRTV => _impl.GetCurrentBackBufferRTV();

    public ITextureView DepthBufferDSV => _impl.GetDepthBufferDSV();
    
    internal SwapChain(IntPtr handle) : base(handle)
    {
        if(PlatformUtils.IsWasm)
            _impl = new SwapChainWebImpl(handle);
        else
            _impl = new SwapChainDefaultImpl(handle);
    }

    protected override void Release()
    {
        _impl = new SwapChainDisposedImpl();
        base.Release();
    }

    public void Present(uint syncInterval = 1) => _impl.Present(syncInterval);

    public void Resize(uint newWidth, uint newHeight, SurfaceTransform newTransform)
        => _impl.Resize(newWidth, newHeight, newTransform);

    public unsafe void SetFullscreenMode(DisplayModeAttribs displayMode)
        => _impl.SetFullscreenMode(displayMode);
    
    public void SetWindowedMode() => _impl.SetWindowedMode();
    
    public void SetMaximumFrameLatency(uint maxLatency) 
        => _impl.SetMaximumFrameLatency(maxLatency);
}
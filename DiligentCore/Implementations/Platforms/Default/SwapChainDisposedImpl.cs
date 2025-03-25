namespace Diligent.Platforms.Default;

internal class SwapChainDisposedImpl : ISwapChainImpl
{
    public SwapChainDesc GetDesc() => throw new ObjectDisposedException(nameof(ISwapChain));

    public ITextureView GetCurrentBackBufferRTV() => throw new ObjectDisposedException(nameof(ISwapChain));

    public ITextureView GetDepthBufferDSV() => throw new ObjectDisposedException(nameof(ISwapChain));
    public void Present(uint syncInterval) => throw new ObjectDisposedException(nameof(ISwapChain));

    public void Resize(uint newWidth, uint newHeight, SurfaceTransform transform) =>
        throw new ObjectDisposedException(nameof(ISwapChain));

    public void SetFullscreenMode(DisplayModeAttribs attribs) => throw new ObjectDisposedException(nameof(ISwapChain));

    public void SetWindowedMode() => throw new ObjectDisposedException(nameof(ISwapChain));

    public void SetMaximumFrameLatency(uint value) => throw new ObjectDisposedException(nameof(ISwapChain));
}
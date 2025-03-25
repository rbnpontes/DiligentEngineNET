namespace Diligent.Platforms.Default;

internal interface ISwapChainImpl
{
    public SwapChainDesc GetDesc();
    public ITextureView GetCurrentBackBufferRTV();
    public ITextureView GetDepthBufferDSV();
    public void Present(uint syncInterval);
    public void Resize(uint newWidth, uint newHeight, SurfaceTransform transform);
    public void SetFullscreenMode(DisplayModeAttribs attribs);
    public void SetWindowedMode();
    public void SetMaximumFrameLatency(uint value);
}
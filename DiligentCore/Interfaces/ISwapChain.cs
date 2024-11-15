using System.Runtime.Versioning;

namespace Diligent;

public interface ISwapChain : IDiligentObject
{
    SwapChainDesc Desc { get; }
    ITextureView CurrentBackBufferRTV { get; }
    
    ITextureView DepthBufferDSV { get; }
    
    void Present(uint syncInterval = 1);

    void Resize(uint newWidth, uint newHeight,
        SurfaceTransform newTransform = SurfaceTransform.Optimal);

    void SetFullscreenMode(DisplayModeAttribs displayMode);

    void SetWindowedMode();

    void SetMaximumFrameLatency(uint maxLatency);
}
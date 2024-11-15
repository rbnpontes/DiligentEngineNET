namespace Diligent;

public partial class SwapChainDesc
{
    public SwapChainDesc()
    {
        _data.ColorBufferFormat = TextureFormat.Rgba8UnormSrgb;
        _data.DepthBufferFormat = TextureFormat.D32Float;
        _data.Usage = SwapChainUsageFlags.RenderTarget;
        _data.PreTransform = SurfaceTransform.Optimal;
        if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS())
            _data.BufferCount = 3;
        else
            _data.BufferCount = 2;
        _data.DefaultDepthValue = 1.0f;
        _data.IsPrimary = true;
    }
}
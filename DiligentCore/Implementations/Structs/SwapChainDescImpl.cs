namespace Diligent;

public partial class SwapChainDesc
{
    public SwapChainDesc()
    {
        _data.ColorBufferFormat = TextureFormat.TexFormatRgba8UnormSrgb;
        _data.DepthBufferFormat = TextureFormat.TexFormatD32Float;
        _data.Usage = SwapChainUsageFlags.SwapChainUsageRenderTarget;
        _data.PreTransform = SurfaceTransform.SurfaceTransformOptimal;
        if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS())
            _data.BufferCount = 3;
        else
            _data.BufferCount = 2;
        _data.DefaultDepthValue = 1.0f;
        _data.IsPrimary = true;
    }
}
namespace Diligent;

public interface ITextureView : IDeviceObject
{
    TextureViewDesc Desc { get; }
    ISampler? Sampler { get; }
    ITexture? Texture { get; }
}
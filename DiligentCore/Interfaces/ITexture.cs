namespace Diligent;

public interface ITexture : IDeviceObject, IDeviceObjectState, IDeviceObjectNativeHandle
{
    TextureDesc Desc { get; }
    SparseTextureProperties SparseProperties { get; }
    ITextureView CreateView(TextureViewDesc viewDesc);
    ITextureView? GetDefaultView(TextureViewType viewType);
}
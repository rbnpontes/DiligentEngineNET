using System.Runtime.InteropServices;

namespace Diligent;

internal unsafe partial class TextureView : ITextureView
{
    public new TextureViewDesc Desc
    {
        get
        {
            var data = (TextureViewDesc.__Internal*)Interop.texture_view_get_desc(Handle).ToPointer();
            var result = TextureViewDesc.FromInternalStruct(*data);
            result.Name = Marshal.PtrToStringAnsi(data->Name) ?? string.Empty;
            return result;
        }
    }

    public ISampler? Sampler
    {
        get
        {
            var ptr = Interop.texture_view_get_sampler(Handle);
            return NativeObjectRegistry.GetOrCreate<ISampler>(() => new UnDisposableSampler(ptr), ptr);
        }
    }

    public ITexture? Texture
    {
        get
        {
            var ptr = Interop.texture_view_get_texture(Handle);
            return NativeObjectRegistry.GetOrCreate<ITexture>(() => new UnDisposableTexture(ptr), ptr);
        }
    }
    
    public TextureView(IntPtr handle) : base(handle){}
}

internal class UnDisposableTextureView(IntPtr handle) : TextureView(handle)
{
    protected override void Release()
    {
        // do nothing.
    }
}
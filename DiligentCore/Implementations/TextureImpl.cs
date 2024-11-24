using System.Runtime.InteropServices;
using Diligent.Utils;

namespace Diligent;

internal unsafe partial class Texture(IntPtr handle) : DeviceObject(handle), ITexture
{
    public ResourceState State
    {
        get => Interop.texture_get_state(Handle);
        set => Interop.texture_set_state(Handle, value);
    }

    public new TextureDesc Desc
    {
        get
        {
            var data = (TextureDesc.__Internal*)Interop.texture_get_desc(Handle);
            var result = TextureDesc.FromInternalStruct(*data);
            result.Name = Marshal.PtrToStringAnsi(data->Name) ?? string.Empty;
            return result;
        }
    }

    public ulong NativeHandle => Interop.texture_get_native_handle(Handle);

    public SparseTextureProperties SparseProperties
    {
        get
        {
            var data = (SparseTextureProperties.__Internal*)Interop.texture_get_sparse_properties(Handle).ToPointer();
            return SparseTextureProperties.FromInternalStruct(*data);
        }
    }

    public ITextureView CreateView(TextureViewDesc viewDesc)
    {
        using var strAlloc = new StringAllocator();
        var viewDescData = TextureViewDesc.GetInternalStruct(viewDesc);
        viewDescData.Name = strAlloc.Acquire(viewDesc.Name);

        var viewPtr = IntPtr.Zero;
        Interop.texture_create_view(Handle, new IntPtr(&viewDescData), new IntPtr(&viewPtr));
        return DiligentObjectsFactory.CreateTextureView(viewPtr);
    }

    public ITextureView? GetDefaultView(TextureViewType viewType)
    {
        var viewPtr = Interop.texture_get_default_view(Handle, viewType);
        return NativeObjectRegistry.GetOrCreate<ITextureView>(() => new UnDisposableTextureView(viewPtr), viewPtr);
    }
}

internal class UnDisposableTexture(IntPtr handle) : Texture(handle)
{
    protected override void Release()
    {
    }
}
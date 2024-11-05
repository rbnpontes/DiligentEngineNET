namespace Diligent;

public partial class DataBlob : IDataBlob
{
    public ulong Size
    {
        get => GetSize();
        set => Resize(value);
    }

    public IntPtr DataPtr => GetDataPtr(0);

    public DataBlob() : base() {}
    
    internal DataBlob(IntPtr ptr) : base(ptr){}
    
    public void Resize(ulong newSize)
    {
        Interop.data_blob_resize(Handle, newSize);
    }

    public ulong GetSize()
    {
        return Interop.data_blob_get_size(Handle);
    }

    public IntPtr GetDataPtr(ulong offset)
    {
        return Interop.data_blob_get_data_ptr(Handle, offset);
    }
}
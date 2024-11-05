namespace Diligent;

public interface IDataBlob : IDiligentObject
{
    ulong Size { get; set; }
    IntPtr DataPtr { get; }
    void Resize(ulong newSize);
    ulong GetSize();
    IntPtr GetDataPtr(ulong offset);
}
namespace Diligent;

public interface IFileStream : IDiligentObject
{
    bool IsValid { get; }
    ulong Size { get; }
    bool Read(IntPtr data, ulong bufferSize);
    bool Read<T>(ref T data) where T : unmanaged;
    bool Read<T>(ref T[] data) where T : unmanaged;
    void ReadBlob(IDataBlob blob);
    bool Write(IntPtr data, ulong size);
    bool Write<T>(ref T data) where T : unmanaged;
    bool Write<T>(T[] data) where T : unmanaged;
}
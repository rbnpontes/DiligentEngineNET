using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Diligent;

internal partial class FileStream : IFileStream
{
    public bool IsValid => Interop.file_stream_is_valid(Handle);
    public ulong Size => Interop.file_stream_get_size(Handle);
    public ulong Pos => Interop.file_stream_get_pos(Handle);

    public FileStream() : base() {}
    internal FileStream(IntPtr handle) : base(handle){}
    
    public bool Read(IntPtr data, ulong bufferSize)
    {
        return Interop.file_stream_read(Handle, data, bufferSize);
    }

    public unsafe bool Read<T>(ref T data) where T : unmanaged
    {
        fixed(T* dataPtr = &data)
            return Read(new IntPtr(dataPtr), (ulong)Unsafe.SizeOf<T>());
    }

    public unsafe bool Read<T>(ref T[] data) where T : unmanaged
    {
        fixed (T* dataPtr = &MemoryMarshal.GetArrayDataReference(data))
            return Read(new IntPtr(dataPtr), (ulong)(data.Length * Unsafe.SizeOf<T>()));
    }

    public void ReadBlob(IDataBlob blob)
    {
        Interop.file_stream_read_blob(Handle, blob.Handle);
    }

    public bool Write(IntPtr data, ulong size)
    {
        return Interop.file_stream_write(Handle, data, size);
    }

    public unsafe bool Write<T>(ref T data) where T : unmanaged
    {
        fixed (T* dataPtr = &data)
            return Write(new IntPtr(dataPtr), (ulong)Unsafe.SizeOf<T>());
    }

    public unsafe bool Write<T>(T[] data) where T : unmanaged
    {
        fixed (T* dataPtr = &data.AsSpan().GetPinnableReference())
            return Write(new IntPtr(dataPtr), (ulong)(Unsafe.SizeOf<T>() * data.Length));
    }

    public bool SetPos(ulong offset, int origin)
    {
        return Interop.file_stream_set_pos(Handle, offset, origin);
    }
}
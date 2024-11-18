using System.Runtime.InteropServices;
using Diligent.Utils;

namespace Diligent;

internal unsafe partial class BufferView : IBufferView
{
    public new BufferViewDesc Desc
    {
        get
        {
            var desc = (BufferViewDesc.__Internal*)Interop.buffer_view_get_desc(Handle);
            var result = BufferViewDesc.FromInternalStruct(*desc);
            result.Name = Marshal.PtrToStringAnsi(desc->Name) ?? string.Empty;
            return result;
        }
    }

    public IBuffer Buffer
    {
        get
        {
            var bufferPtr = Interop.buffer_view_get_buffer(Handle);
            return DiligentObjectsFactory.CreateBuffer(bufferPtr);
        }
    }
    
    public BufferView(IntPtr handle) : base(handle){}
}

internal class UndisposableBufferView : BufferView
{
    public UndisposableBufferView(IntPtr handle) : base(handle) {}

    protected override void Release()
    {
    }
}
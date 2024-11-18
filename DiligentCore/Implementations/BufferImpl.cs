using System.Runtime.InteropServices;
using Diligent.Utils;

namespace Diligent;

unsafe partial class Buffer : IBuffer
{
    public new BufferDesc Desc
    {
        get
        {
            var desc = (BufferDesc.__Internal*)Interop.buffer_get_desc(Handle);
            var result = BufferDesc.FromInternalStruct(*desc);
            result.Name = Marshal.PtrToStringAnsi(desc->Name) ?? string.Empty;
            return result;
        }
    }

    public ulong NativeHandle => Interop.buffer_get_native_handle(Handle);

    public ResourceState State
    {
        get => Interop.buffer_get_state(Handle);
        set => Interop.buffer_set_state(Handle, value);
    }

    public MemoryProperties MemoryProperties => Interop.buffer_get_memory_properties(Handle);

    public SparseBufferProperties SparseProperties
    {
        get
        {
            var bufferProps = (SparseBufferProperties.__Internal*)Interop.buffer_get_sparse_properties(Handle);
            return SparseBufferProperties.FromInternalStruct(*bufferProps);
        }
    }

    public Buffer(IntPtr handle) : base(handle){}
    
    public IBufferView CreateView(BufferViewDesc viewDesc)
    {
        using var strAlloc = new StringAllocator();
        var viewDescData = BufferViewDesc.GetInternalStruct(viewDesc);
        viewDescData.Name = strAlloc.Acquire(viewDesc.Name);
        
        var bufferViewPtr = IntPtr.Zero;
        Interop.buffer_create_view(Handle, 
            new IntPtr(&viewDescData), 
            new IntPtr(&bufferViewPtr));

        return DiligentObjectsFactory.CreateBufferView(bufferViewPtr);
    }

    public IBufferView? GetDefaultView(BufferViewType viewType)
    {
        var bufferViewPtr = Interop.buffer_get_default_view(Handle, viewType);
        if (bufferViewPtr == IntPtr.Zero)
            return null;
        return NativeObjectRegistry.GetOrCreate(() => new UndisposableBufferView(bufferViewPtr), bufferViewPtr);
    }

    public void FlushMappedRange(ulong startOffset, ulong size)
    {
        Interop.buffer_flush_mapped_range(Handle, startOffset, size);
    }

    public void InvalidateMappedRange(ulong startOffset, ulong size)
    {
        Interop.buffer_invalidate_mapped_range(Handle, startOffset, size);
    }
}
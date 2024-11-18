namespace Diligent;

public interface IBuffer : IDeviceObject
{
    new BufferDesc Desc { get; }
    ulong NativeHandle { get; }
    ResourceState State { get; set; }
    MemoryProperties MemoryProperties { get; }
    SparseBufferProperties SparseProperties { get; }
    IBufferView CreateView(BufferViewDesc viewDesc);
    IBufferView? GetDefaultView(BufferViewType viewType);
    void FlushMappedRange(ulong startOffset, ulong size);
    void InvalidateMappedRange(ulong startOffset, ulong size);
    
}
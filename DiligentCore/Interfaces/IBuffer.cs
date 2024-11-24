namespace Diligent;

public interface IBuffer : IDeviceObject, IDeviceObjectState, IDeviceObjectNativeHandle
{
    new BufferDesc Desc { get; }
    MemoryProperties MemoryProperties { get; }
    SparseBufferProperties SparseProperties { get; }
    IBufferView CreateView(BufferViewDesc viewDesc);
    IBufferView? GetDefaultView(BufferViewType viewType);
    void FlushMappedRange(ulong startOffset, ulong size);
    void InvalidateMappedRange(ulong startOffset, ulong size);
    
}
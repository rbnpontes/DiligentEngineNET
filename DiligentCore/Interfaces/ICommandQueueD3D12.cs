namespace Diligent;

// TODO: add UpdateTileMappings method
public interface ICommandQueueD3D12 : ICommandQueue
{
    IntPtr D3D12CommandQueue { get; }
    IntPtr D3D12CommandQueueDesc { get; }
    
    ulong Submit(IntPtr[] d3d12CommandLists);
    void EnqueueSignal(IntPtr fence, ulong value);
    void WaitFence(IntPtr fence, ulong value);
}
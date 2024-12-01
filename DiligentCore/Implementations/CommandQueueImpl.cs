namespace Diligent;

internal partial class CommandQueue(IntPtr handle) : DiligentObject(handle), ICommandQueue
{
    public ulong NextFenceValue => Interop.command_queue_get_next_fence_value(Handle);
    public ulong CompletedFenceValue => Interop.command_queue_get_completed_fence_value(Handle);
    
    public ulong WaitForIdle()
    {
        return Interop.command_queue_wait_for_idle(Handle);
    }
}
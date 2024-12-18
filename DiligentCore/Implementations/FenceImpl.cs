using Diligent.Utils;

namespace Diligent;

internal partial class Fence : IFence
{
    public new FenceDesc Desc => DiligentDescFactory.GetFenceDesc(Handle);

    public ulong CompletedValue => Interop.fence_get_completed_value(Handle);
    internal Fence(IntPtr handle) : base(handle)
    {
        Console.WriteLine("Fence Created");
    }
    
    public void Signal(ulong value)
    {
        Interop.fence_signal(Handle, value);
    }

    public void Wait(ulong value)
    {
        Interop.fence_wait(Handle, value);
    }
}
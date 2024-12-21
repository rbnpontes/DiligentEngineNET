namespace Diligent.Null;

public class NullFence(IntPtr handle) : NullDeviceObject(handle), IFence
{
    public new FenceDesc Desc => new()
    {
        Name = nameof(NullFence),
    };
    public ulong CompletedValue { get; set; }
    
    public void Signal(ulong value)
    {
    }

    public void Wait(ulong value)
    {
    }
}
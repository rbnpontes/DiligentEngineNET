namespace Diligent;

public interface IFence : IDeviceObject
{
    new FenceDesc Desc { get; }
    ulong CompletedValue { get; }
    void Signal(ulong value);
    void Wait(ulong value);
}
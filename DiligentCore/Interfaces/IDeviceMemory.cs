namespace Diligent;

public interface IDeviceMemory : IDeviceObject
{
    new DeviceMemoryDesc Desc { get; }
    ulong Capacity { get; }
    bool Resize(ulong newSize);
    bool IsCompatible(IDeviceObject resource);
}
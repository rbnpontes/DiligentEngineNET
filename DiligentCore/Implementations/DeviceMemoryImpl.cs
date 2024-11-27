using Diligent.Utils;

namespace Diligent;

internal partial class DeviceMemory(IntPtr handle) : DeviceObject(handle), IDeviceMemory
{
    public DeviceMemoryDesc Desc => DiligentDescFactory.GetDeviceMemoryDesc(
        Interop.device_memory_get_desc(Handle)
    );

    public ulong Capacity => Interop.device_memory_get_capacity(Handle);

    public bool Resize(ulong newSize)
    {
        return Interop.device_memory_resize(Handle, newSize);
    }

    public bool IsCompatible(IDeviceObject resource)
    {
        return Interop.device_memory_is_compatible(Handle, resource.Handle);
    }
}
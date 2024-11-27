using System.Runtime.InteropServices;
using Diligent.Utils;

namespace Diligent;

internal partial class DeviceObject(IntPtr handle) : DiligentObject(handle), IDeviceObject
{
    public DeviceObjectAttribs Desc => DiligentDescFactory.GetDeviceObjectAttribs(
        Interop.device_object_get_desc(Handle)
    );

    public int UniqueId => Interop.device_object_get_unique_id(Handle);

    public IDiligentObject? UserData
    {
        get => DiligentObjectsFactory.TryGetOrCreateObject(
            Interop.device_object_get_user_data(Handle)
        );
        set => Interop.device_object_set_user_data(Handle, value?.Handle ?? IntPtr.Zero);
    }
}

public sealed class UnknownDeviceObject : UnknownObject, IDeviceObject
{
    public DeviceObjectAttribs Desc => DiligentDescFactory.GetDeviceObjectAttribs(
        DeviceObject.Interop.device_object_get_desc(Handle)
    );

    public int UniqueId => DeviceObject.Interop.device_object_get_unique_id(Handle);

    public IDiligentObject? UserData
    {
        get => DiligentObjectsFactory.TryGetOrCreateObject(
            DeviceObject.Interop.device_object_get_user_data(Handle)
        );
        set => DeviceObject.Interop.device_object_set_user_data(Handle, value?.Handle ?? IntPtr.Zero);
    }

    internal UnknownDeviceObject(IntPtr handle) : base(handle)
    {
    }
}
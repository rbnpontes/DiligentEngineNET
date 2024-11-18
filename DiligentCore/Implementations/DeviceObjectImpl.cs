using System.Runtime.InteropServices;

namespace Diligent;

internal unsafe partial class DeviceObject : IDeviceObject
{
    public DeviceObjectAttribs Desc
    {
        get
        {
            var desc = (DeviceObjectAttribs.__Internal*)Interop.device_object_get_desc(Handle);
            var result = DeviceObjectAttribs.FromInternalStruct(*desc);
            result.Name = Marshal.PtrToStringAnsi(desc->Name) ?? string.Empty;
            return result;
        }
    }

    public int UniqueId
    {
        get => Interop.device_object_get_unique_id(Handle);
    }

    public IDiligentObject? UserData
    {
        get
        {
            var userDataPtr = Interop.device_object_get_user_data(Handle);
            NativeObjectRegistry.TryGetObject<IDiligentObject>(userDataPtr, out var result);
            return result;
        }
        set => Interop.device_object_set_user_data(Handle, value?.Handle ?? IntPtr.Zero);
    }
    
    public DeviceObject(IntPtr handle) : base(handle){}
}
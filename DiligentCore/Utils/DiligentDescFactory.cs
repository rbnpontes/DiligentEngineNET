using System.Runtime.InteropServices;

namespace Diligent.Utils;

public static unsafe class DiligentDescFactory
{
    public static DeviceObjectAttribs GetDeviceObjectAttribs(IntPtr handle)
    {
        var desc = (DeviceObjectAttribs.__Internal*)DeviceObject.Interop.device_object_get_desc(handle);
        var result = DeviceObjectAttribs.FromInternalStruct(*desc);
        result.Name = Marshal.PtrToStringAnsi(handle) ?? string.Empty;
        return result;
    }
}
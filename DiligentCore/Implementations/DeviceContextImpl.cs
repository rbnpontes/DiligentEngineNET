namespace Diligent;

internal partial class DeviceContext : IDeviceContext
{
    public DeviceContext() : base(){}
    internal DeviceContext(IntPtr handle) : base(handle){}
}
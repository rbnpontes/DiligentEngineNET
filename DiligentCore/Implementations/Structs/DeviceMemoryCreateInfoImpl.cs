namespace Diligent;

public partial class DeviceMemoryCreateInfo
{
    private IDeviceObject[] _compatibleResources = [];
    public IDeviceObject[] CompatibleResources
    {
        get => _compatibleResources;
        set
        {
            _compatibleResources = value;
            _data.NumResources = (uint)value.Length;
        }
    }
}
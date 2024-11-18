namespace Diligent;

public interface IDeviceObject : IDiligentObject
{
    DeviceObjectAttribs Desc { get; }
    int UniqueId { get; }
    IDiligentObject? UserData { get; set; }
}
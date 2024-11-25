namespace Diligent;

public interface IBottomLevelAS : IDeviceObject, IDeviceObjectNativeHandle, IDeviceObjectState
{
    new BottomLevelASDesc Desc { get; }
    uint ActualGeometryCount { get; }
    ScratchBufferSizes ScratchBufferSizes { get; }
    uint GetGeometryDescIndex(string name);
    uint GetGeometryIndex(string name);
    
}
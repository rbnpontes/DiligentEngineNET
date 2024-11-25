namespace Diligent;

public interface ITopLevelAS : IDeviceObject, IDeviceObjectNativeHandle, IDeviceObjectState
{
    new TopLevelASDesc Desc { get; }
    TLASBuildInfo BuildInfo { get; }
    ScratchBufferSizes ScratchBufferSizes { get; }
    TLASInstanceDesc GetInstanceDesc(string name);
}
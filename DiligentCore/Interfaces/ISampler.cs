namespace Diligent;

public interface ISampler : IDeviceObject
{
    new SamplerDesc Desc { get; }
}
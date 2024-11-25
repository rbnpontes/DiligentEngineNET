namespace Diligent;

public interface IFramebuffer : IDeviceObject
{
    new FramebufferDesc Desc { get; }
}
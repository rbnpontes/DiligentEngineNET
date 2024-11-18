namespace Diligent;

public interface IBufferView : IDeviceObject
{
    public new BufferViewDesc Desc { get; }
}
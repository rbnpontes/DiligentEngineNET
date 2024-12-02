namespace Diligent;

public partial class BufferDesc
{
    public BufferDesc()
    {
        _data.Usage = Usage.Default;
        _data.ImmediateContextMask = 1;
    }
}
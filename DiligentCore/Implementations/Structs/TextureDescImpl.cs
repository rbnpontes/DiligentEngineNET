namespace Diligent;

public partial class TextureDesc
{
    public TextureDesc()
    {
        _data.ImmediateContextMask = 1;
        _data.SampleCount = 1;
        _data.MipLevels = 1;
        _data.ArraySizeOrDepth = 1;
    }
}
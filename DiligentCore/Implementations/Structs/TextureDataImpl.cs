namespace Diligent;

public partial class TextureData
{
    private TextureSubResData[] _subRes = [];
    public TextureSubResData[] SubResources
    {
        get => _subRes;
        set
        {
            _subRes = value;
            _data.NumSubresources = (uint)_subRes.Length;
        }
    }
}
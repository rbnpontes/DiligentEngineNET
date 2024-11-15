namespace Diligent;

public partial class SamplerDesc
{
    public SamplerDesc()
    {
        _data.MinFilter = _data.MagFilter = _data.MipFilter = FilterType.Linear;
        _data.AddressU = _data.AddressV = _data.AddressW = TextureAddressMode.Clamp;
        _data.ComparisonFunc = ComparisonFunction.Never;
        _data.MaxLOD = float.MaxValue;
    }
}
namespace Diligent;

public partial class SamplerDesc
{
    public SamplerDesc()
    {
        _data.MinFilter = _data.MagFilter = _data.MipFilter = FilterType.FilterTypeLinear;
        _data.AddressU = _data.AddressV = _data.AddressW = TextureAddressMode.TextureAddressClamp;
        _data.ComparisonFunc = ComparisonFunction.ComparisonFuncNever;
        _data.MaxLOD = float.MaxValue;
    }
}
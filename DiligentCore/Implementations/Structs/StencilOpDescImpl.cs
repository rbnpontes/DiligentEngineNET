namespace Diligent;

public partial class StencilOpDesc
{
    public StencilOpDesc()
    {
        _data.StencilFailOp = _data.StencilDepthFailOp = _data.StencilPassOp = StencilOp.Keep;
        _data.StencilFunc = ComparisonFunction.Always;
    }
}
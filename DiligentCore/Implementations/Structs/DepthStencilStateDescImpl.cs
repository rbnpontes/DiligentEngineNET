namespace Diligent;

public partial class DepthStencilStateDesc
{
    public DepthStencilStateDesc()
    {
        _data.DepthEnable = _data.DepthWriteEnable = true;
        _data.DepthFunc = ComparisonFunction.Less;
        _data.StencilReadMask = _data.StencilWriteMask = 0xFF;
    }
}
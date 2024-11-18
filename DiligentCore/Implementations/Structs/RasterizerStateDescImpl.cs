namespace Diligent;

public partial class RasterizerStateDesc
{
    public RasterizerStateDesc()
    {
        _data.FillMode = FillMode.Solid;
        _data.CullMode = CullMode.Back;
        _data.DepthClipEnable = true;
    }
}
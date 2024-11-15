namespace Diligent;

public partial class RenderTargetBlendDesc
{
    public RenderTargetBlendDesc()
    {
        _data.SrcBlend = BlendFactor.One;
        _data.DestBlend = BlendFactor.Zero;
        _data.BlendOp = BlendOperation.Add;
        _data.SrcBlendAlpha = BlendFactor.One;
        _data.DestBlendAlpha = BlendFactor.Zero;
        _data.BlendOpAlpha = BlendOperation.Add;
        _data.LogicOp = LogicOperation.Noop;
        _data.RenderTargetWriteMask = ColorMask.All;
    }
}
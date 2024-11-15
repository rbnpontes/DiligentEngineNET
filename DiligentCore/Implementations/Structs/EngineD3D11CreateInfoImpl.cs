namespace Diligent;

public partial class EngineD3D11CreateInfo
{
    public EngineD3D11CreateInfo()
    {
        GraphicsAPIVersion = new Version(11, 0);
    }
    public override void SetValidation(ValidationLevel level)
    {
        base.SetValidation(level);
        
        _data.D3D11ValidationFlags = D3d11ValidationFlags.None;
        if (level >= ValidationLevel.N2)
            _data.D3D11ValidationFlags |= D3d11ValidationFlags.VerifyCommittedResourceRelevance;
    }
}
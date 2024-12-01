namespace Diligent;

public partial class DispatchComputeAttribs
{
    public DispatchComputeAttribs()
    {
        _data.ThreadGroupCountX =
            _data.ThreadGroupCountY =
                _data.ThreadGroupCountZ = 1;
        _data.MtlThreadGroupSizeX =
            _data.MtlThreadGroupSizeY =
                _data.MtlThreadGroupSizeZ = 1;
    }

    public DispatchComputeAttribs(uint groupsX, uint groupsY, uint groupsZ = 1)
    {
        _data.ThreadGroupCountX = groupsX;
        _data.ThreadGroupCountY = groupsY;
        _data.ThreadGroupCountZ = groupsZ;
    }
}
namespace Diligent;

public partial class StateTransitionDesc
{
    public StateTransitionDesc()
    {
        _data.MipLevelsCount = Constants.RemainingMipLevels;
        _data.ArraySliceCount = Constants.RemainingArraySlices;
    }
}
namespace Diligent;

public partial class DispatchTileAttribs
{
    public DispatchTileAttribs()
    {
        _data.ThreadsPerTileX = _data.ThreadsPerTileY = 1;
    }

    public DispatchTileAttribs(uint threadsX, uint threadsY, DrawFlags flags = DrawFlags.None)
    {
        _data.ThreadsPerTileX = threadsX;
        _data.ThreadsPerTileY = threadsY;
        _data.Flags = flags;
    }
}
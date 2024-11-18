namespace Diligent;

public partial class PipelineStateCacheCreateInfo
{
    private byte[] _cacheData = [];

    /// <summary>
    /// Use this if you have a byte array of pipeline state cache.
    /// </summary>
    public byte[] CacheDataBytes
    {
        get => _cacheData;
        set
        {
            _cacheData = value;
            _data.CacheDataSize = (uint)value.Length;
        }
    }
}
namespace Diligent;

public interface IQuery : IDeviceObject
{
    new QueryDesc Desc { get; }
    bool GetData(out QueryDataOcclusion data, bool autoInvalidate = true);
    bool GetData(out QueryDataBinaryOcclusion data, bool autoInvalidate = true);
    bool GetData(out QueryDataTimestamp data, bool autoInvalidate = true);
    bool GetData(out QueryDataPipelineStatistics data, bool autoInvalidate = true);
    void Invalidate();
}
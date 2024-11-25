using System.Runtime.CompilerServices;
using Diligent.Utils;

namespace Diligent;

internal unsafe partial class Query(IntPtr handle) : DeviceObject(handle), IQuery
{
    public new QueryDesc Desc => DiligentDescFactory.GetQueryDesc(Handle);

    public bool GetData(out QueryDataOcclusion data, bool autoInvalidate = true)
    {
        QueryDataOcclusion.__Internal dataInternal = new();
        var result = Interop.query_get_data(Handle, 
            new IntPtr(&dataInternal), 
            (uint)Unsafe.SizeOf<QueryDataOcclusion.__Internal>(),
            autoInvalidate);
        data = QueryDataOcclusion.FromInternalStruct(dataInternal);
        return result;
    }

    public bool GetData(out QueryDataBinaryOcclusion data, bool autoInvalidate = true)
    {
        QueryDataBinaryOcclusion.__Internal dataInternal = new();
        var result = Interop.query_get_data(Handle, 
            new IntPtr(&dataInternal), 
            (uint)Unsafe.SizeOf<QueryDataBinaryOcclusion.__Internal>(),
            autoInvalidate);
        data = QueryDataBinaryOcclusion.FromInternalStruct(dataInternal);
        return result;
    }

    public bool GetData(out QueryDataTimestamp data, bool autoInvalidate = true)
    {
        QueryDataTimestamp.__Internal dataInternal = new();
        var result = Interop.query_get_data(Handle, 
            new IntPtr(&dataInternal), 
            (uint)Unsafe.SizeOf<QueryDataTimestamp.__Internal>(),
            autoInvalidate);
        data = QueryDataTimestamp.FromInternalStruct(dataInternal);
        return result;
    }

    public bool GetData(out QueryDataPipelineStatistics data, bool autoInvalidate = true)
    {
        QueryDataPipelineStatistics.__Internal dataInternal = new();
        var result = Interop.query_get_data(Handle, 
            new IntPtr(&dataInternal), 
            (uint)Unsafe.SizeOf<QueryDataPipelineStatistics.__Internal>(),
            autoInvalidate);
        data = QueryDataPipelineStatistics.FromInternalStruct(dataInternal);
        return result;
    }

    public void Invalidate()
    {
        Interop.query_invalidate(Handle);
    }
}
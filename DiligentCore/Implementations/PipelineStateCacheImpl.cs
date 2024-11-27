using Diligent.Utils;

namespace Diligent;

internal unsafe partial class PipelineStateCache(IntPtr handle) : DeviceObject(handle), IPipelineStateCache
{
    public IDataBlob GetData()
    {
        var blobPtr = IntPtr.Zero;
        Interop.pipeline_state_cache_get_data(Handle, new IntPtr(&blobPtr));
        return DiligentObjectsFactory.CreateDataBlob(blobPtr);
    }
}
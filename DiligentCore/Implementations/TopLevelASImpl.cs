using Diligent.Utils;

namespace Diligent;

internal partial class TopLevelAS(IntPtr handle) : DeviceObject(handle), ITopLevelAS
{
    public ulong NativeHandle => Interop.top_level_as_get_native_handle(Handle);

    public ResourceState State
    {
        get => Interop.top_level_as_get_state(Handle);
        set => Interop.top_level_as_set_state(Handle, value);
    }
    
    public new TopLevelASDesc Desc => DiligentDescFactory.GetTopLevelASDesc(
        Interop.top_level_as_get_desc(Handle)    
    );

    public TLASBuildInfo BuildInfo => DiligentDescFactory.GetTLASBuildInfo(
        Interop.top_level_as_get_build_info(Handle)
    );

    public ScratchBufferSizes ScratchBufferSizes => DiligentDescFactory.GetStratchBufferSizes(
        Interop.top_level_as_get_scratch_buffer_sizes(Handle)
    );
    
    public TLASInstanceDesc GetInstanceDesc(string name)
    {
        using var strAlloc = new StringAllocator();
        return DiligentDescFactory.GetTLASInstanceDesc(
            Interop.top_level_as_get_instance_desc(Handle, strAlloc.Acquire(name))
        );
    }
}
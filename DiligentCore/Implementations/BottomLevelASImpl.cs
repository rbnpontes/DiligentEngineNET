using Diligent.Utils;

namespace Diligent;

internal partial class BottomLevelAS(IntPtr handle) : DeviceObject(handle), IBottomLevelAS
{
    public new BottomLevelASDesc Desc => DiligentDescFactory.GetBottomLevelASDesc(
        Interop.bottom_level_as_get_desc(Handle)
    );
    public ScratchBufferSizes ScratchBufferSizes => DiligentDescFactory.GetStratchBufferSizes(
        Interop.bottom_level_as_get_scratch_buffer_sizes(Handle)
    );
    public uint ActualGeometryCount => Interop.bottom_level_as_get_actual_geometry_count(Handle);
    public ulong NativeHandle => Interop.bottom_level_as_get_native_handle(Handle);
    public ResourceState State
    {
        get => Interop.bottom_level_as_get_state(Handle);
        set => Interop.bottom_level_as_set_state(Handle, value);
    }

    public uint GetGeometryDescIndex(string name)
    {
        using var strAlloc = new StringAllocator();
        return Interop.bottom_level_as_get_geometry_desc_index(
            Handle,
            strAlloc.Acquire(name)
        );
    }

    public uint GetGeometryIndex(string name)
    {
        using var strAlloc = new StringAllocator();
        return Interop.bottom_level_as_get_geometry_index(
            Handle,
            strAlloc.Acquire(name)
        );
    }
}
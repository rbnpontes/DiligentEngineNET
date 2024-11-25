using Diligent.Utils;

namespace Diligent;

internal unsafe partial class ShaderBindingTable(IntPtr handle) : DeviceObject(handle), IShaderBindingTable
{
    public new ShaderBindingTableDesc Desc => DiligentDescFactory.GetShaderBindingTableDesc(Handle);
    public bool Verify(VerifySbtFlags flags)
    {
        return Interop.shader_binding_table_verify(Handle, flags);
    }

    public void Reset(IPipelineState pipelineState)
    {
        Interop.shader_binding_table_reset(Handle, pipelineState.Handle);
    }

    public void ResetHitGroups()
    {
        Interop.shader_binding_table_reset_hit_groups(Handle);
    }

    public void BindRayGenShader(string shaderGroupName, IntPtr data, uint dataSize)
    {
        using var strAlloc = new StringAllocator();
        Interop.shader_binding_table_bind_ray_gen_shader(
            Handle,
            strAlloc.Acquire(shaderGroupName),
            data,
            dataSize
        );
    }

    public void BindRayGenShader(string shaderGroupName, byte[] data)
    {
        var span = data.AsSpan();
        fixed(void* dataPtr = span)
            BindRayGenShader(shaderGroupName, new IntPtr(dataPtr), (uint)data.Length);
    }

    public void BindMissShader(string shaderGroupName, uint missIndex, IntPtr data, uint dataSize)
    {
        using var strAlloc = new StringAllocator();
        Interop.shader_binding_table_bind_miss_shader(Handle,
            strAlloc.Acquire(shaderGroupName),
            missIndex,
            data,
            dataSize);
    }

    public void BindMissShader(string shaderGroupName, uint missIndex, byte[] data)
    {
        var span = data.AsSpan();
        fixed (void* dataPtr = span)
            BindMissShader(shaderGroupName, missIndex, new IntPtr(dataPtr), (uint)data.Length);
    }

    public void BindHitGroupForGeometry(ITopLevelAS tlas, string instanceName, string geometryName, uint rayOffsetInHitGroupIndex,
        string shaderGroupName, IntPtr data, uint dataSize)
    {
        using var strAlloc = new StringAllocator();
        Interop.shader_binding_table_bind_hit_group_for_geometry(Handle, 
            tlas.Handle,
            strAlloc.Acquire(instanceName),
            strAlloc.Acquire(geometryName),
            rayOffsetInHitGroupIndex,
            strAlloc.Acquire(shaderGroupName),
            data,
            dataSize);
    }

    public void BindHitGroupForGeometry(ITopLevelAS tlas, string instanceName, string geometryName, uint rayOffsetInHitGroupIndex,
        string shaderGroupName, byte[] data)
    {
        var span = data.AsSpan();
        fixed(void* dataPtr = span)
            BindHitGroupForGeometry(tlas, 
                instanceName,
                geometryName, 
                rayOffsetInHitGroupIndex,
                shaderGroupName, 
                new IntPtr(dataPtr), 
                (uint)data.Length);
    }

    public void BindHitGroupByIndex(uint bindingIndex, string shaderGroupName, IntPtr data, uint dataSize)
    {
        using var strAlloc = new StringAllocator();
        Interop.shader_binding_table_bind_hit_group_by_index(Handle, 
            bindingIndex,
            strAlloc.Acquire(shaderGroupName),
            data,
            dataSize);
    }

    public void BindHitGroupByIndex(uint bindingIndex, string shaderGroupName, byte[] data)
    {
        var span = data.AsSpan();
        fixed(void* dataPtr = span)
            BindHitGroupByIndex(bindingIndex, shaderGroupName, new IntPtr(dataPtr), (uint)data.Length);
    }

    public void BindHitGroupForInstance(ITopLevelAS tlas, string instanceName, uint rayOffsetInHitGroupIndex,
        string shaderGroupName, IntPtr data, uint dataSize)
    {
        using var strAlloc = new StringAllocator();
        Interop.shader_binding_table_bind_hit_group_for_instance(Handle, 
            tlas.Handle,
            strAlloc.Acquire(instanceName),
            rayOffsetInHitGroupIndex,
            strAlloc.Acquire(shaderGroupName),
            data,
            dataSize);
    }

    public void BindHitGroupForInstance(ITopLevelAS tlas, string instanceName, uint rayOffsetInHitGroupIndex,
        string shaderGroupName, byte[] data)
    {
        var span = data.AsSpan();
        fixed (void* dataPtr = span)
            BindHitGroupForInstance(tlas, instanceName, rayOffsetInHitGroupIndex, shaderGroupName, new IntPtr(dataPtr), (uint)data.Length);
    }

    public void BindHitGroupForTLAS(ITopLevelAS tlas, uint rayOffsetInHitGroupIndex, string shaderGroupName, IntPtr data,
        uint dataSize)
    {
        using var strAlloc = new StringAllocator();
        Interop.shader_binding_table_bind_hit_group_for_tlas(Handle,
            tlas.Handle,
            rayOffsetInHitGroupIndex,
            strAlloc.Acquire(shaderGroupName),
            data,
            dataSize);
    }

    public void BindHitGroupForTLAS(ITopLevelAS tlas, uint rayOffsetInHitGroupIndex, string shaderGroupName, byte[] data)
    {
        var span = data.AsSpan();
        fixed (void* dataPtr = span)
            BindHitGroupForTLAS(tlas, rayOffsetInHitGroupIndex, shaderGroupName, new IntPtr(dataPtr), (uint)data.Length);
    }

    public void BindCallableShader(string name, uint callableIndex, IntPtr data, uint dataSize)
    {
        using var strAlloc = new StringAllocator();
        Interop.shader_binding_table_bind_callable_shader(Handle,
            strAlloc.Acquire(name),
            callableIndex,
            data,
            dataSize);
    }

    public void BindCallableShader(string name, uint callableIndex, byte[] data)
    {
        var span = data.AsSpan();
        fixed (void* dataPtr = span)
            BindCallableShader(name, callableIndex, new IntPtr(dataPtr), (uint)data.Length);
    }
}
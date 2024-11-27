using Diligent.Utils;

namespace Diligent;

internal unsafe partial class PipelineState(IntPtr handle) : DeviceObject(handle), IPipelineState
{
    public new PipelineStateDesc Desc => DiligentDescFactory.GetPipelineStateDesc(
        Interop.pipeline_state_get_desc(Handle)
    );

    public GraphicsPipelineDesc? GraphicsPipelineDesc => Desc.PipelineType != PipelineType.Graphics 
        ? null 
        : DiligentDescFactory.GetGraphicsPipelineDesc(
            Interop.pipeline_state_get_graphics_pipeline_desc(Handle)    
        );

    public RayTracingPipelineDesc? RayTracingPipelineDesc => Desc.PipelineType != PipelineType.RayTracing
        ? null
        : DiligentDescFactory.GetRayTracingPipelineDesc(
            Interop.pipeline_state_get_ray_tracing_pipeline_desc(Handle)    
        );

    public TilePipelineDesc? TilePipelineDesc => Desc.PipelineType != PipelineType.Tile
        ? null
        : DiligentDescFactory.GetTilePipelineDesc(
            Interop.pipeline_state_get_tile_pipeline_desc(Handle)
        );

    public uint ResourceSignatureCount => Interop.pipeline_state_get_resource_signature_count(Handle);
    
    public void BindStaticResources(ShaderType shaderStageFlags, IResourceMapping resourceMapping, BindShaderResourcesFlags flags)
    {
        Interop.pipeline_state_bind_static_resources(Handle, 
            shaderStageFlags, 
            resourceMapping.Handle,
            flags);
    }

    public uint GetStaticVariableCount(ShaderType shaderType)
    {
        return Interop.pipeline_state_get_static_variable_count(Handle, shaderType);
    }

    public IShaderResourceVariable? GetStaticVariableByName(ShaderType shaderType, string name)
    {
        using var strAlloc = new StringAllocator();
        var ptr = Interop.pipeline_state_get_static_variable_by_name(Handle, shaderType, strAlloc.Acquire(name));
        return ptr == IntPtr.Zero ? null : DiligentObjectsFactory.CreateShaderResourceVariable(ptr);
    }

    public IShaderResourceVariable GetStaticVariableByIndex(ShaderType shaderType, uint index)
    {
        return DiligentObjectsFactory.CreateShaderResourceVariable(
            Interop.pipeline_state_get_static_variable_by_index(Handle, shaderType, index)
        );
    }

    public IShaderResourceBinding CreateShaderResourceBinding(bool initStaticResources = false)
    {
        var ptr = IntPtr.Zero;
        Interop.pipeline_state_create_shader_resource_binding(Handle, new IntPtr(&ptr), initStaticResources);
        return DiligentObjectsFactory.CreateShaderResourceBinding(ptr);
    }

    public void InitializeStaticSRBResources(IShaderResourceBinding shaderResourceBinding)
    {
        Interop.pipeline_state_initialize_static_srbresources(Handle, shaderResourceBinding.Handle);
    }

    public void CopyStaticResources(IPipelineState dstPipeline)
    {
        Interop.pipeline_state_copy_static_resources(Handle, dstPipeline.Handle);
    }

    public bool IsCompatibleWith(IPipelineState pipelineState)
    {
        return Interop.pipeline_state_is_compatible_with(Handle, pipelineState.Handle);
    }

    public IPipelineResourceSignature GetResourceSignature(uint index)
    {
        var ptr = Interop.pipeline_state_get_resource_signature(Handle, index);
        return DiligentObjectsFactory.CreatePipelineResourceSignature(ptr);
    }

    public PipelineStateStatus GetStatus(bool waitForCompletion = false)
    {
        return Interop.pipeline_state_get_status(Handle, waitForCompletion);
    }
}
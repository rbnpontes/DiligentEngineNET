using Diligent.Utils;

namespace Diligent;

internal unsafe partial class PipelineResourceSignature(IntPtr handle) : DeviceObject(handle), IPipelineResourceSignature
{
    public new PipelineResourceSignatureDesc Desc => DiligentDescFactory.GetPipelineResourceSignatureDesc(
        Interop.pipeline_resource_signature_get_desc(Handle)
    );
    public IShaderResourceBinding CreateShaderResourceBinding(bool initStaticResources = false)
    {
        var srbPtr = IntPtr.Zero;
        Interop.pipeline_resource_signature_create_shader_resource_binding(Handle, new IntPtr(&srbPtr), initStaticResources);
        return DiligentObjectsFactory.CreateShaderResourceBinding(srbPtr);
    }

    public void BindStaticResources(ShaderType shaderStageFlags, IResourceMapping resourceMapping, BindShaderResourcesFlags flags)
    {
        Interop.pipeline_resource_signature_bind_static_resources(Handle,shaderStageFlags, resourceMapping.Handle, flags);
    }

    public IShaderResourceVariable? GetStaticVariableByName(ShaderType shaderType, string name)
    {
        using var strAlloc = new StringAllocator();
        var varPtr =
            Interop.pipeline_resource_signature_get_static_variable_by_name(Handle, shaderType, strAlloc.Acquire(name));
        return varPtr == IntPtr.Zero ? null : DiligentObjectsFactory.CreateShaderResourceVariable(varPtr);
    }

    public IShaderResourceVariable GetStaticVariableByIndex(ShaderType shaderType, uint index)
    {
        var varPtr = Interop.pipeline_resource_signature_get_static_variable_by_index(Handle, shaderType, index);
        return DiligentObjectsFactory.CreateShaderResourceVariable(varPtr);
    }

    public uint GetStaticVariableCount(ShaderType shaderType)
    {
        return Interop.pipeline_resource_signature_get_static_variable_count(Handle, shaderType);
    }

    public void InitializeStaticSRBResources(IShaderResourceBinding shaderResourceBinding)
    {
        Interop.pipeline_resource_signature_initialize_static_srbresources(Handle, shaderResourceBinding.Handle);
    }

    public void CopyStaticResources(IPipelineResourceSignature dstSignature)
    {
        Interop.pipeline_resource_signature_copy_static_resources(Handle, dstSignature.Handle);
    }

    public bool IsCompatibleWith(IPipelineResourceSignature pipelineResourceSignature)
    {
        return Interop.pipeline_resource_signature_is_compatible_with(Handle, pipelineResourceSignature.Handle);
    }
}

internal class UnDisposablePipelineResourceSignature(IntPtr handle) : PipelineResourceSignature(handle)
{
    protected override void Release()
    {
    }
}
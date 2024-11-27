using Diligent.Utils;

namespace Diligent;

internal partial class ShaderResourceBinding(IntPtr handle) : DiligentObject(handle), IShaderResourceBinding
{
    public IPipelineResourceSignature? PipelineResourceSignature
    {
        get
        {
            var ptr = Interop.shader_resource_binding_get_pipeline_resource_signature(Handle);
            if (ptr == IntPtr.Zero)
                return null;
            return NativeObjectRegistry.GetOrCreate<IPipelineResourceSignature>(
                () => new UnDisposablePipelineResourceSignature(ptr), ptr);
        }
    }

    public bool IsStaticResourcesInitialized => Interop.shader_resource_binding_static_resources_initialized(Handle);

    protected override void Release()
    {
    }

    public void BindResources(ShaderType shaderStageFlags, IResourceMapping resMapping, BindShaderResourcesFlags flags)
    {
        Interop.shader_resource_binding_bind_resources(Handle, shaderStageFlags, resMapping.Handle, flags);
    }

    public ShaderResourceVariableTypeFlags CheckResources(ShaderType shaderStageFlags, IResourceMapping resMapping,
        BindShaderResourcesFlags flags)
    {
        return Interop.shader_resource_binding_check_resources(Handle, shaderStageFlags, resMapping.Handle, flags);
    }

    public IShaderResourceVariable? GetVariableByName(ShaderType shaderType, string name)
    {
        using var strAlloc = new StringAllocator();
        var varPtr = Interop.shader_resource_binding_get_variable_by_name(Handle, shaderType, strAlloc.Acquire(name));
        return varPtr == IntPtr.Zero ? null : DiligentObjectsFactory.CreateShaderResourceVariable(Handle);
    }

    public uint GetVariableCount(ShaderType shaderType)
    {
        return Interop.shader_resource_binding_get_variable_count(Handle, shaderType);
    }

    public IShaderResourceVariable GetVariableByIndex(ShaderType shaderType, uint index)
    {
        var varPtr = Interop.shader_resource_binding_get_variable_by_index(Handle, shaderType, index);
        return DiligentObjectsFactory.CreateShaderResourceVariable(varPtr);
    }
}
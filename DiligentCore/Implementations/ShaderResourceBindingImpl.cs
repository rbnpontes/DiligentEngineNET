namespace Diligent;

internal partial class ShaderResourceBinding(IntPtr handle) : DiligentObject(handle), IShaderResourceBinding
{
    public IPipelineResourceSignature PipelineResourceSignature { get; }
    public bool IsStaticResourcesInitialized { get; }

    protected override void Release()
    {
    }

    public void BindResources(ShaderType shaderStageFlags, IResourceMapping resMapping, BindShaderResourcesFlags flags)
    {
        throw new NotImplementedException();
    }

    public ShaderResourceVariableTypeFlags CheckResources(ShaderType shaderStageFlags, IResourceMapping resMapping,
        BindShaderResourcesFlags flags)
    {
        throw new NotImplementedException();
    }

    public IShaderResourceVariable? GetVariableByName(ShaderType shaderType, string name)
    {
        throw new NotImplementedException();
    }

    public uint GetVariableCount(ShaderType shaderType)
    {
        throw new NotImplementedException();
    }

    public IShaderResourceVariable GetVariableByIndex(ShaderType shaderType, uint index)
    {
        throw new NotImplementedException();
    }
}
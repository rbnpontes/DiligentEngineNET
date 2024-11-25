namespace Diligent;

public interface IShaderResourceBinding : IDiligentObject
{
    IPipelineResourceSignature PipelineResourceSignature { get; }
    bool IsStaticResourcesInitialized { get; }

    void BindResources(ShaderType shaderStageFlags, IResourceMapping resMapping, BindShaderResourcesFlags flags);

    ShaderResourceVariableTypeFlags CheckResources(ShaderType shaderStageFlags, IResourceMapping resMapping,
        BindShaderResourcesFlags flags);

    IShaderResourceVariable? GetVariableByName(ShaderType shaderType, string name);
    
    uint GetVariableCount(ShaderType shaderType);
    
    IShaderResourceVariable GetVariableByIndex(ShaderType shaderType, uint index);
}
namespace Diligent;

public interface IPipelineResourceSignature : IDiligentObject
{
    new PipelineResourceSignatureDesc Desc { get; }
    IShaderResourceBinding CreateShaderResourceBinding(bool initStaticResources = false);

    void BindStaticResources(ShaderType shaderStageFlags, IResourceMapping resourceMapping,
        BindShaderResourcesFlags flags);

    IShaderResourceVariable? GetStaticVariableByName(ShaderType shaderType, string name);

    IShaderResourceVariable GetStaticVariableByIndex(ShaderType shaderType, uint index);

    uint GetStaticVariableCount(ShaderType shaderType);

    void InitializeStaticSRBResources(IShaderResourceBinding shaderResourceBinding);

    void CopyStaticResources(IPipelineResourceSignature dstSignature);

    bool IsCompatibleWith(IPipelineResourceSignature pipelineResourceSignature);
}
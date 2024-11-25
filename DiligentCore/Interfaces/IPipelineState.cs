namespace Diligent;

public interface IPipelineState : IDiligentObject
{
     PipelineStateDesc Desc { get; }
     GraphicsPipelineDesc? GraphicsPipelineDesc { get; }
     RayTracingPipelineDesc? RayTracingPipelineDesc { get; }
     TilePipelineDesc? TilePipelineDesc { get; }
     uint ResourceSignatureCount { get; }

     void BindStaticResources(ShaderType shaderStageFlags, IResourceMapping resourceMapping,
          BindShaderResourcesFlags flags);

     uint GetStaticVariableCount(ShaderType shaderType);
     IShaderResourceVariable? GetStaticVariableByName(ShaderType shaderType, string name);
     IShaderResourceVariable GetStaticVariableByIndex(ShaderType shaderType, uint index);
     IShaderResourceBinding CreateShaderResourceBinding(bool initStaticResources = false);
     void InitializeStaticSRBResources(IShaderResourceBinding shaderResourceBinding);
     void CopyStaticResources(IPipelineState dstPipeline);
     bool IsCompatibleWith(IPipelineState pipelineState);
     IPipelineResourceSignature GetResourceSignature(uint index);
     PipelineStateStatus GetStatus(bool waitForCompletion = false);
}
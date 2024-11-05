namespace Diligent;

public interface IDearchiver : IDiligentObject
{
    uint ContentVersion { get; }
    bool LoadArchive(IDataBlob dataBlob, uint contentVersion = 0, bool makeCopy = false);
    IShader UnpackShader(ShaderUnpackInfo unpackInfo);
    IPipelineState UnpackPipelineState(PipelineStateUnpackInfo unpackInfo);
    IPipelineResourceSignature UnpackResourceSignature(ResourceSignatureUnpackInfo unpackInfo);
    IRenderPass UnpackRenderPass(RenderPassUnpackInfo unpackInfo);
    bool Store(out IDataBlob? dataBlob);
    void Reset();
}
namespace Diligent;

public partial class PipelineStateCreateInfo
{
    internal PipelineStateCreateInfo() {}
    internal PipelineStateCreateInfo(PipelineType pipelineType)
    {
        _data.PSODesc.PipelineType = pipelineType;
    }
}
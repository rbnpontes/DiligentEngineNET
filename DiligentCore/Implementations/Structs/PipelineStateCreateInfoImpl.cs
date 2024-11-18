namespace Diligent;

public partial class PipelineStateCreateInfo
{
    private IPipelineResourceSignature[] _resourceSignatures = [];
    public IPipelineResourceSignature[] ResourceSignatures
    {
        get => _resourceSignatures;
        set
        {
            _resourceSignatures = value;
            _data.ResourceSignaturesCount = (uint)value.Length;
        }
    }

    internal PipelineStateCreateInfo() {}
    internal PipelineStateCreateInfo(PipelineType pipelineType)
    {
        _data.PSODesc.PipelineType = pipelineType;
    }
}
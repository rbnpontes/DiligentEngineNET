namespace Diligent;

public partial class PipelineResourceSignatureDesc
{
    public PipelineResourceSignatureDesc()
    {
        CombinedSamplerSuffix = "_sampler";
        _data.SRBAllocationGranularity = 1;
    }
}
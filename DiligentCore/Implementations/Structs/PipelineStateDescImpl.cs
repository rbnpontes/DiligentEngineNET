namespace Diligent;

public partial class PipelineStateDesc
{
     public PipelineStateDesc()
     {
          _data.PipelineType = PipelineType.Graphics;
          _data.SRBAllocationGranularity = 1;
          _data.ImmediateContextMask = 1;
     }
}
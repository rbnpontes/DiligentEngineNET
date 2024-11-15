namespace Diligent;

public partial class GraphicsPipelineDesc
{
    public GraphicsPipelineDesc()
    {
        _data.SampleMask = 0xFFFFFFFF;
        _data.PrimitiveTopology = PrimitiveTopology.TriangleList;
        _data.NumViewports = 1;
    }
}
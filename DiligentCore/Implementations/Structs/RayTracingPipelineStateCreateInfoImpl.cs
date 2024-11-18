namespace Diligent;

public partial class RayTracingPipelineStateCreateInfo() : PipelineStateCreateInfo(PipelineType.RayTracing)
{
    private RayTracingGeneralShaderGroup[] _generalShaders = [];
    private RayTracingTriangleHitShaderGroup[] _triangleHitShaders = [];
    private RayTracingProceduralHitShaderGroup[] _proceduralHitShaders = [];

    public RayTracingGeneralShaderGroup[] GeneralShaders
    {
        get => _generalShaders;
        set
        {
            _generalShaders = value;
            _data.GeneralShaderCount = (uint)value.Length;
        }
    }

    public RayTracingTriangleHitShaderGroup[] TriangleHitShaders
    {
        get => _triangleHitShaders;
        set
        {
            _triangleHitShaders = value;
            _data.TriangleHitShaderCount = (uint)value.Length;
        }
    }

    public RayTracingProceduralHitShaderGroup[] ProceduralHitShaderGroup
    {
        get => _proceduralHitShaders;
        set
        {
            _proceduralHitShaders = value;
            _data.ProceduralHitShaderCount = (uint)value.Length;
        }
    }
}
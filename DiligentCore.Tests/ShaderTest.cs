namespace Diligent.Tests;

[TestFixture]
[NonParallelizable]
public class ShaderTest : BaseRenderTest
{
    private readonly string _testShader = @"
cbuffer Constants
{
    float4x4 g_world_view_proj;
    float4 g_color;
};

struct VertexInput
{
    float3 position : ATTRIB0;
    float3 normal : ATTRIB1;
};

struct PixelInput
{
    float4 position: SV_POSITION;
    float4 color: COLOR;
};

PixelInput VS(VertexInput input)
{
    PixelInput output;
    float4 pos = float4(input.position, 1.0);
    output.position = mul(pos, g_world_view_proj);
    output.color = g_color;
#if defined(DEBUG)
    output.color = output.color + float(0.001);
#endif
    return output;
}";
    
    [Test]
    public void MustCreate()
    {
        var createInfo = new ShaderCreateInfo()
        {
            Desc = new ShaderDesc()
            {
                  Name = "Test Shader",
                  ShaderType = ShaderType.Vertex,
            },
            Source = _testShader,
            EntryPoint = "VS",
            SourceLanguage = ShaderSourceLanguage.Hlsl,
            Macros = [new ShaderMacro("DEBUG", "1")]
        };
        using var shader = Device.CreateShader(createInfo);
        
        Assert.That(shader, Is.Not.Null);
    }

    [Test]
    public void MustGetDesc()
    {
        var createInfo = new ShaderCreateInfo()
        {
            Desc = new ShaderDesc()
            {
                Name = "Test Shader",
                ShaderType = ShaderType.Vertex,
            },
            Source = _testShader,
            EntryPoint = "VS",
            SourceLanguage = ShaderSourceLanguage.Hlsl,
            Macros = [new ShaderMacro("DEBUG", "1")],
        };
        using var shader = Device.CreateShader(createInfo);
        var testDesc = createInfo.Desc;

        Assert.That(createInfo.Desc.Name, Is.EqualTo(testDesc.Name));
    }

    [Test]
    public void MustGetResourceCount()
    {
        var createInfo = new ShaderCreateInfo()
        {
            Desc = new ShaderDesc()
            {
                Name = "Test Shader",
                ShaderType = ShaderType.Vertex,
            },
            Source = _testShader,
            EntryPoint = "VS",
            SourceLanguage = ShaderSourceLanguage.Hlsl,
            LoadConstantBufferReflection = true,
            Macros = [new ShaderMacro("DEBUG", "1")],
        };
        using var shader = Device.CreateShader(createInfo);
        var cbufferDesc = shader.GetConstantBufferDesc(0);
        Assert.Multiple(() =>
        {
            Assert.That(shader.ResourceCount, Is.GreaterThan(0));
            Assert.That(cbufferDesc, Is.Not.Null);
            Assert.That(cbufferDesc.Variables, Is.Not.Empty);
        });
    }
}
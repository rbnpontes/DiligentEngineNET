namespace Diligent.Tests;

[TestFixture]
[NonParallelizable]
public class GraphicsPipelineStateTest : BasePipelineStateTest
{
    [Test]
    public void MustCreate()
    {
        using var pipeline = CreateGraphicsPipelineState();
        Assert.That(pipeline, Is.Not.Null);
    }

    [Test]
    public void MustCreateShaderResourceBinding()
    {
        using var pipeline = CreateGraphicsPipelineState();
        using var srb = pipeline.CreateShaderResourceBinding();
        Assert.Multiple(() =>
        {
            Assert.That(pipeline, Is.Not.Null);
            Assert.That(srb, Is.Not.Null);
        });
    }
    
    [Test]
    public void MustGetDesc()
    {
        using var pipeline = CreateGraphicsPipelineState();
        var desc = pipeline.Desc;
        Assert.That(desc.Name, Is.EqualTo("Graphics Pipeline"));
    }

    [Test]
    public void MustGetStaticVariableCount()
    {
        using var pipeline = CreateGraphicsPipelineState(ShaderResourceVariableType.Static);
        Assert.Multiple(() =>
        {
            Assert.That(pipeline.GetStaticVariableCount(ShaderType.Vertex), Is.GreaterThan(0));
            Assert.That(pipeline.GetStaticVariableCount(ShaderType.Pixel), Is.GreaterThan(0));
        });
    }

    [Test]
    public void MustGetVariableByName()
    {
        using var pipeline = CreateGraphicsPipelineState(ShaderResourceVariableType.Static);
        var vertexVar = pipeline.GetStaticVariableByName(ShaderType.Vertex, "GlobalConstants");
        var pixelVar = pipeline.GetStaticVariableByName(ShaderType.Pixel, "GlobalConstants");
        
        Assert.Multiple(() =>
        {
            Assert.That(vertexVar, Is.Not.Null);
            Assert.That(pixelVar, Is.Not.Null);
        });
    }

    [Test]
    public void MustGetStaticVariableByIndex()
    {
        using var pipeline = CreateGraphicsPipelineState(ShaderResourceVariableType.Static);
        var vertexVar = pipeline.GetStaticVariableByIndex(ShaderType.Vertex, 0);
        var pixelVar = pipeline.GetStaticVariableByIndex(ShaderType.Pixel, 0);
        
        Assert.Multiple(() =>
        {
            Assert.That(vertexVar, Is.Not.Null);
            Assert.That(pixelVar, Is.Not.Null);
        });
    }
}
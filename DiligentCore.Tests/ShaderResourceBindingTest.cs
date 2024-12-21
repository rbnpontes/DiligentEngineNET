namespace Diligent.Tests;

[TestFixture]
[NonParallelizable]
public class ShaderResourceBindingTest : BasePipelineStateTest
{
    [Test]
    public void MustGetPipelineResourceSignature()
    {
        using var pipeline = CreateGraphicsPipelineState();
        using var srb = pipeline.CreateShaderResourceBinding();
        var pipelineRes = srb.PipelineResourceSignature;

        Assert.That(pipelineRes, Is.Not.Null);
    }

    [Test]
    public void MustGetVariableCount([Values(ShaderType.Vertex, ShaderType.Pixel)] ShaderType type)
    {
        using var pipeline = CreateGraphicsPipelineState();
        using var srb = pipeline.CreateShaderResourceBinding(true);
        Assert.That(srb.GetVariableCount(type), Is.GreaterThan(0));
    }

    [Test]
    public void MustGetVariableByIndex([Values(ShaderType.Vertex, ShaderType.Pixel)] ShaderType type)
    {
        using var pipeline = CreateGraphicsPipelineState();
        using var srb = pipeline.CreateShaderResourceBinding(true);
        var shaderVar = srb.GetVariableByIndex(type, 0);
        Assert.That(shaderVar, Is.Not.Null);
    }
    
    [Test]
    public void MustGetVariableByName([Values(ShaderType.Vertex, ShaderType.Pixel)] ShaderType type)
    {
        using var pipeline = CreateGraphicsPipelineState();
        using var srb = pipeline.CreateShaderResourceBinding(true);
        var shaderVar = srb.GetVariableByName(type, "GlobalConstants");
        Assert.That(shaderVar, Is.Not.Null);
    }
}
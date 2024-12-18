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
}
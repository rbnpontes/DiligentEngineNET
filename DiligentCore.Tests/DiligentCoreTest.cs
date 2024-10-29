namespace Diligent.Tests;

public class DiligentCoreTest
{
    [Test]
    public void MustGetFactoryD3D11()
    {
        if (!OperatingSystem.IsWindows())
        {
            Assert.Pass();
            return;
        }
        
        Assert.That(DiligentCore.GetEngineFactoryD3D11(), Is.Not.Null);
    }

    [Test]
    public void MustGetFactoryD3D12()
    {
        if(!OperatingSystem.IsWindows())
            return;
        
        Assert.That(DiligentCore.GetEngineFactoryD3D12(), Is.Not.Null);
    }
    
    [Test]
    public void MustGetFactoryVulkan()
    {
        Assert.That(DiligentCore.GetEngineFactoryVk(), Is.Not.Null);
    }
    
    [Test]
    public void MustGetFactoryOpenGL()
    {
        Assert.That(DiligentCore.GetEngineFactoryOpenGL(), Is.Not.Null);
    }
}
using Moq;

namespace Diligent.Tests;

public class EngineFactoryTest
{
    private IEngineFactory GetFactory()
    {
        IEngineFactory? result;
        if (OperatingSystem.IsWindows())
             result = DiligentCore.GetEngineFactoryD3D11();
        else
            result = DiligentCore.GetEngineFactoryVk();
        return result ?? throw new NullReferenceException();
    }
    [Test]
    public void MustEnumerateAdapters()
    {
        var engineFactory = GetFactory();
        var adapters = engineFactory.EnumerateAdapters(new Version(11, 0));
        Assert.That(adapters, Has.No.Empty);
        
        foreach (var adapter in adapters)
            Assert.That(adapter.Description, Is.Not.Empty);
    }

    [Test]
    public void MustSetMessageCallback()
    {
        var engineFactory = GetFactory();
        var messageCallback = new Mock<DebugMessageCallbackDelegate>();
        engineFactory.SetMessageCallback(messageCallback.Object);
    }

    [Test]
    public void MustGetApiInfo()
    {
        var engineFactory = GetFactory();
        var apiInfo = engineFactory.APIInfo;
        
        Assert.That(apiInfo, Is.Not.Null);
    }

    [Test]
    public void MustSetBreakOnError()
    {
        var engineFactory = GetFactory();
        engineFactory.SetBreakOnError(true);
        engineFactory.SetBreakOnError(false);
    }
    
    [Test]
    public void MustCreateDataBlob()
    {
        var factory = GetFactory();
        using var dataBlob = factory.CreateDataBlob(1024);
        Assert.That(dataBlob, Is.Not.Null);
    }
    
    
}
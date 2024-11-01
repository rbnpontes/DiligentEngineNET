using Moq;

namespace Diligent.Tests;

[TestFixture]
public class EngineFactoryTest : BaseFactoryTest
{
    private struct TestStruct
    {
        public int A;
        public IntPtr B;
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
    public void MustCreateDefaultShaderSourceStreamFactory()
    {
        var engineFactory = GetFactory();
        using (engineFactory.CreateDefaultShaderSourceStreamFactory(Environment.CurrentDirectory)){}
        
        Assert.Pass();
    }
    
    [Test]
    public void MustCreateDataBlob()
    {
        var factory = GetFactory();
        var dataBlob = factory.CreateDataBlob(1024);
        Assert.That(dataBlob, Is.Not.Null);
        dataBlob.Dispose();

        var values = new[] { 1, 2, 3, 4 };
        dataBlob = factory.CreateDataBlob<int>(values);
        Assert.That(dataBlob, Is.Not.Null);
        dataBlob.Dispose();

        var data = new TestStruct();
        data.A = 20;
        data.B = new IntPtr(0xFF);
        dataBlob = factory.CreateDataBlob(ref data);
        Assert.That(dataBlob, Is.Not.Null);
        dataBlob.Dispose();
    }
}
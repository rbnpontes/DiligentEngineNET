namespace Diligent.Tests;

[TestFixture]
[NonParallelizable]
public class DearchiverTest : BaseFactoryTest
{
    [Test]
    public void MustCreate()
    {
        using var factory = GetFactory();
        using var dearchiver = factory.CreateDearchiver(new DearchiverCreateInfo());
        
        Assert.That(dearchiver, Is.Not.Null);
    }

    [Test]
    public void MustGetContentVersion()
    {
        using var factory = GetFactory();
        using var dearchiver = factory.CreateDearchiver(new DearchiverCreateInfo());
        
        Assert.That(dearchiver.ContentVersion, Is.Not.EqualTo(0));
    }

    [Test]
    public void MustStore()
    {
        using var factory = GetFactory();
        using var dearchiver = factory.CreateDearchiver(new DearchiverCreateInfo());
        
        dearchiver.Store(out var dataBlob);
        dataBlob?.Dispose();
        
        Assert.That(dataBlob, Is.Not.Null);
    }

    [Test]
    public void MustReset()
    {
        using var factory = GetFactory();
        using var dearchiver = factory.CreateDearchiver(new DearchiverCreateInfo());
        
        dearchiver.Reset();
        
        Assert.Pass();
    }
}
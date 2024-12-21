namespace Diligent.Tests;

[TestFixture]
[NonParallelizable]
public class DiligentObjectTest : BaseFactoryTest
{
    [Test]
    public void MustGetHandle()
    {
        var factory = GetFactory();
        Assert.That(factory.Handle, Is.Not.EqualTo(IntPtr.Zero));
    }

    [Test]
    public void MustGetReferenceCounters()
    {
        var factory = GetFactory();
        Assert.That(factory.ReferenceCounters, Is.Not.Null);
    }

    [Test]
    public void MustReturnSameInstanceOfReferenceCounters()
    {
        var factory = GetFactory();
        var firstRef = factory.ReferenceCounters;
        var secondRef = factory.ReferenceCounters;
        Assert.That(firstRef, Is.EqualTo(secondRef));
    }

    [Test]
    public void MustDispose()
    {
        var factory = GetFactory();
        factory.Dispose();
        Assert.Pass();
    }
}
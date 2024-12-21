using System.Runtime.InteropServices;

namespace Diligent.Tests;

[TestFixture]
[NonParallelizable]
public unsafe class DataBlobTest : BaseFactoryTest
{
    private struct TestStruct
    {
        public int A;
        public IntPtr B;
    }

    [Test]
    public void MustCreateFromGivenSize()
    {
        using var factory = GetFactory();
        using var dataBlob = factory.CreateDataBlob(32);

        Assert.That(dataBlob, Is.Not.Null);
        Assert.That(dataBlob.DataPtr, Is.Not.EqualTo(IntPtr.Zero));
    }
    
    [Test]
    public void MustCreateFromGivenPointer()
    {
        var testStruct = new TestStruct();
        testStruct.A = 10;
        testStruct.B = new IntPtr(0x2048);

        using var factory = GetFactory();
        using var dataBlob = factory.CreateDataBlob((ulong)Marshal.SizeOf<TestStruct>(), new IntPtr(&testStruct));
        
        Assert.That(dataBlob, Is.Not.Null);
        
        var dataPtr = (TestStruct*)dataBlob.DataPtr.ToPointer();
        
        Assert.That(dataPtr->A, Is.EqualTo(testStruct.A));
        Assert.That(dataPtr->B, Is.EqualTo(testStruct.B));
    }
    
    [Test]
    public void MustCreateWithArrayPrimitive()
    {
        var arr = new[] { 1, 2, 3 };
        using var factory = GetFactory();
        using var dataBlob = factory.CreateDataBlob<int>(arr);
        var data = new ReadOnlySpan<int>(dataBlob.DataPtr.ToPointer(), arr.Length);
        
        Assert.That(data.ToArray(), Is.EquivalentTo(arr));
    }
    
    [Test]
    public void MustCreateWithStruct()
    {
        var testStruct = new TestStruct();
        testStruct.A = 20;
        testStruct.B = new IntPtr(0x1024);
        
        using var factory = GetFactory();
        using var dataBlob = factory.CreateDataBlob(ref testStruct);
        var dataPtr = (TestStruct*)dataBlob.DataPtr.ToPointer();
        
        Assert.That(dataPtr->A, Is.EqualTo(testStruct.A));
        Assert.That(dataPtr->B, Is.EqualTo(testStruct.B));
    }

    [Test]
    public void MustGetDataPtr()
    {
        using var factory = GetFactory();
        using var dataBlob = factory.CreateDataBlob(32);
        
        Assert.That(dataBlob.GetDataPtr(4), Is.Not.EqualTo(IntPtr.Zero));
    }
    
    [Test]
    public void MustReturnSize()
    {
        using var factory = GetFactory();
        using var dataBlob = factory.CreateDataBlob(1024);
        
        Assert.That(dataBlob.Size, Is.EqualTo(1024));
    }

    [Test]
    public void MustResize()
    {
        using var factory = GetFactory();
        using var dataBlob = factory.CreateDataBlob(1024);
        dataBlob.Resize(2048);
        dataBlob.Resize(64);
        
        Assert.Pass();
    }
}
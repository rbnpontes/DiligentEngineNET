using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Diligent.Tests;

[TestFixture]
public class FileStreamTest : BaseFactoryTest
{
	private readonly string _basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

	private readonly string _testFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "test-file.txt"
    );

    [SetUp]
    public void SetupTest()
    {
        File.WriteAllText(_testFilePath, "Test File");    
    }

    [TearDown]
    public void Cleanup()
    {
        File.Delete(_testFilePath);
    }
    
    [Test]
    public void MustRead()
    {
        var factory = GetFactory();
        using var shaderSrcStreamFactory = factory.CreateDefaultShaderSourceStreamFactory(_basePath);
        using var fileStream = shaderSrcStreamFactory.CreateInputStream("test-file.txt");
        var data = new byte[fileStream.Size];
        
        Assert.That(fileStream.Read(ref data), Is.True);

        var txt = Encoding.Default.GetString(data);
        Assert.That(txt, Is.EqualTo("Test File"));
    }

    struct TestStruct
    {
        public byte T;
        public byte E;
        public byte S;
        public byte T2;
    }
    
    [Test]
    public void MustReadAsStruct()
    {
        var factory = GetFactory();
        using var shaderSrcStreamFactory = factory.CreateDefaultShaderSourceStreamFactory(_basePath);
        using var fileStream = shaderSrcStreamFactory.CreateInputStream("test-file.txt");

        var structData = new TestStruct();
        
        Assert.That(fileStream.Read(ref structData), Is.True);

        var txtBytes = new[] { structData.T, structData.E, structData.S, structData.T2 };
        var txt = Encoding.Default.GetString(txtBytes);
        
        Assert.That(txt, Is.EqualTo("Test"));
    }

    [Test]
    public unsafe void MustReadFromIntPtr()
    {
        var factory = GetFactory();
        using var shaderSrcStreamFactory = factory.CreateDefaultShaderSourceStreamFactory(_basePath);
        using var fileStream = shaderSrcStreamFactory.CreateInputStream("test-file.txt");
        var block = Marshal.AllocHGlobal((int)fileStream.Size);
        
        Assert.That(fileStream.Read(block, fileStream.Size));

        var txt = Encoding.Default.GetString((byte*)block.ToPointer(), (int)fileStream.Size);
        Assert.That(txt, Is.EqualTo("Test File"));
    }

    public void MustReadIntoDataBlob()
    {
        var factory = GetFactory();
        using var shaderSrcStreamFactory = factory.CreateDefaultShaderSourceStreamFactory(_basePath);
        using var fileStream = shaderSrcStreamFactory.CreateInputStream("test-file.txt");
        using var dataBlob = factory.CreateDataBlob(fileStream.Size);
        
        fileStream.ReadBlob(dataBlob);
        
        Assert.Pass();
    }

    [Test]
    public void MustWrite()
    {
        var factory = GetFactory();
        using var shaderSrcStreamFactory = factory.CreateDefaultShaderSourceStreamFactory(_basePath);
        using var fileStream = shaderSrcStreamFactory.CreateInputStream("test-file.txt");

        var bytes = Encoding.Default.GetBytes("Hello World");
        // file stream created from IShaderSourceStreamFactory opens
        // file as readonly, so write method will return internally as 
        // false.
        Assert.That(fileStream.Write(bytes), Is.False);
    }
}
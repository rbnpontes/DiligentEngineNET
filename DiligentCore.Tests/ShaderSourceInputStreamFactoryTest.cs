namespace Diligent.Tests;

[TestFixture]
[NonParallelizable]
public class ShaderSourceInputStreamFactoryTest : BaseFactoryTest
{
    [Test]
    public void MustCreateFileStream()
    {
        var assemblyName = Path.GetFileName(typeof(IShaderSourceInputStreamFactory).Assembly.Location);
        var factory = GetFactory();
        using var shaderSourceStream = factory.CreateDefaultShaderSourceStreamFactory(Environment.CurrentDirectory);
        using (shaderSourceStream.CreateInputStream(assemblyName)){}
        Assert.Pass();
    }

    [Test]
    public void MustCreateFileStreamFromFlags()
    {
        var assemblyName = Path.GetFileName(typeof(IShaderSourceInputStreamFactory).Assembly.Location);
        var factory = GetFactory();
        using var shaderSourceStream = factory.CreateDefaultShaderSourceStreamFactory(Environment.CurrentDirectory);
        using (shaderSourceStream.CreateInputStream(assemblyName, CreateShaderSourceInputStreamFlags.Silent)){}
        Assert.Pass();
    }
}
namespace Diligent.Tests;

[TestFixture]
public class ShaderSourceInputStreamFactoryTest : BaseFactoryTest
{
    [Test]
    public void MustCreateFileStream()
    {
        var assemblyName = Path.GetFileName(typeof(ShaderSourceInputStreamFactory).Assembly.Location);
        var factory = GetFactory();
        using var shaderSourceStream = factory.CreateDefaultShaderSourceStreamFactory(Environment.CurrentDirectory);
        using (shaderSourceStream.CreateInputStream(assemblyName)){}
        Assert.Pass();
    }

    [Test]
    public void MustCreateFileStreamFromFlags()
    {
        var assemblyName = Path.GetFileName(typeof(ShaderSourceInputStreamFactory).Assembly.Location);
        var factory = GetFactory();
        using var shaderSourceStream = factory.CreateDefaultShaderSourceStreamFactory(Environment.CurrentDirectory);
        using (shaderSourceStream.CreateInputStream(assemblyName, CreateShaderSourceInputStreamFlags.CreateShaderSourceInputStreamFlagSilent)){}
        Assert.Pass();
    }
}
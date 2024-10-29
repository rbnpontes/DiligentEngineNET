namespace Diligent.Tests;

public class EngineFactoryD3D11Test
{
    [Test]
    public void MustEnumerateDisplayModes()
    {
        if (!OperatingSystem.IsWindows())
            return;

        var factory = DiligentCore.GetEngineFactoryD3D11();
        var version = new Version(11, 0);
        
        Assert.That(factory, Is.Not.Null);
        var displayModes = factory!.EnumerateDisplayModes(version, 0, 0, TextureFormat.TexFormatRgba8Unorm);
        Assert.That(displayModes, Has.No.Empty);
    }
}
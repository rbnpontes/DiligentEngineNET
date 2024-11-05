using Diligent.Tests.Utils;

namespace Diligent.Tests;

[TestFixture]
[Platform("Win")]
public class EngineFactoryD3D11Test
{
    private IEngineFactoryD3D11 GetFactory()
    {
        var factory = DiligentCore.GetEngineFactoryD3D11() ?? throw new NullReferenceException();
        factory.SetMessageCallback(DebugOutput.MessageCallback);
        return factory;
    }
    [Test]
    public void MustEnumerateDisplayModes()
    {
        using var factory = GetFactory();
        var version = new Version(11, 0);
        
        Assert.That(factory, Is.Not.Null);
        var displayModes = factory.EnumerateDisplayModes(version, 0, 0, TextureFormat.TexFormatRgba8Unorm);
        Assert.That(displayModes, Has.No.Empty);
    }

    [Test]
    public void MustCreateDeviceAndContext()
    {
        using var factory = GetFactory();
        var createInfo = new EngineD3D11CreateInfo()
        {
            EnableValidation = true,
            GraphicsAPIVersion = new Version(11, 0)
        };
        
        (var renderDevice, var deviceContexts) = factory.CreateDeviceAndContexts(createInfo);
        
        Assert.That(renderDevice, Is.Not.Null);
        Assert.That(deviceContexts, Has.Length.GreaterThan(0));

        foreach (var deviceCtx in deviceContexts)
            deviceCtx.Dispose();
        renderDevice.Dispose();
    }

    [Test]
    public void MustCreateDeferredContexts()
    {
        using var factory = GetFactory();
        var createInfo = new EngineD3D11CreateInfo()
        {
            EnableValidation = true,
            NumDeferredContexts = 3
        };
        
        (var renderDevice, var deviceContexts) = factory.CreateDeviceAndContexts(createInfo);
        Assert.That(renderDevice, Is.Not.Null);
        Assert.That(deviceContexts, Has.Length.EqualTo(4));

        foreach (var deviceCtx in deviceContexts)
            deviceCtx.Dispose();
        renderDevice.Dispose();
    }
}
using Diligent.Tests.Utils;

namespace Diligent.Tests;

[TestFixture]
[Platform("Win")]
public class EngineFactoryD3D12Test
{
    private IEngineFactoryD3D12 GetFactory()
    {
        var factory = DiligentCore.GetEngineFactoryD3D12() ?? throw new NullReferenceException();
        factory.SetMessageCallback(DebugOutput.MessageCallback);
        return factory;
    }

    [Test]
    public void MustLoadD3D12()
    {
        using var factory = GetFactory();
        factory.LoadD3D12();
        Assert.Pass();
    }

    [Test]
    public void MustCreateDeviceAndContext()
    {
        using var factory = GetFactory();
        (var renderDevice, var deviceContexts) = factory.CreateDeviceAndContext(new EngineD3D12CreateInfo());

        Assert.That(renderDevice, Is.Not.Null);
        Assert.That(deviceContexts.FirstOrDefault(), Is.Not.Null);

        foreach (var ctx in deviceContexts)
            ctx.Dispose();
        renderDevice.Dispose();
    }

    [Test]
    public void MustCreateDeferredContexts()
    {
        using var factory = GetFactory();
        (var renderDevice, var deviceContexts) = factory.CreateDeviceAndContext(new EngineD3D12CreateInfo()
        {
            NumDeferredContexts = 3
        });

        Assert.That(renderDevice, Is.Not.Null);
        Assert.That(deviceContexts, Has.Length.EqualTo(4));

        foreach (var ctx in deviceContexts)
            ctx.Dispose();
        renderDevice.Dispose();
    }

    [Test]
    public void MustCreateSwapChain()
    {
        using var window = new TestWindow();
        using var factory = GetFactory();
        (var renderDevice, var deviceContexts) = factory.CreateDeviceAndContext(new EngineD3D12CreateInfo());
        var swapChain = factory.CreateSwapChain(
            renderDevice,
            deviceContexts.First(),
            new SwapChainDesc(),
            new FullScreenModeDesc(),
            WindowHandle.CreateWin32Window(window.Handle));
        
        Assert.That(swapChain, Is.Not.Null);
        
        swapChain.Dispose();
        foreach (var ctx in deviceContexts)
            ctx.Dispose();
        renderDevice.Dispose();
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
}
using Diligent.Tests.Utils;

namespace Diligent.Tests;

[TestFixture]
[Platform("Win")]
public class SwapChainTest
{
    [Test]
    public void MustPresent()
    {
        using var window = new TestWindow();
        using var factory = DiligentCore.GetEngineFactoryD3D11();
        (var device, var contexts) = factory.CreateDeviceAndContexts(new EngineD3D11CreateInfo());
        var swapChain = factory.CreateSwapChain(device, contexts[0], new SwapChainDesc(),
            new FullScreenModeDesc(), WindowHandle.CreateWin32Window(window.Handle));

        swapChain.Present();

        swapChain.Dispose();
        foreach (var ctx in contexts)
            ctx.Dispose();
        device.Dispose();

        Assert.Pass();
    }

    [Test]
    public void MustResize()
    {
        using var window = new TestWindow();
        using var factory = DiligentCore.GetEngineFactoryD3D11();
        (var device, var contexts) = factory.CreateDeviceAndContexts(new EngineD3D11CreateInfo());
        var swapChain = factory.CreateSwapChain(device, contexts[0], new SwapChainDesc(),
            new FullScreenModeDesc(), WindowHandle.CreateWin32Window(window.Handle));

        swapChain.Resize(100, 100);

        swapChain.Dispose();
        foreach (var ctx in contexts)
            ctx.Dispose();
        device.Dispose();
    }

    [Test]
    public void MustSetFullScreenMode()
    {
        using var window = new TestWindow();
        using var factory = DiligentCore.GetEngineFactoryD3D11();
        (var device, var contexts) = factory.CreateDeviceAndContexts(new EngineD3D11CreateInfo());
        var swapChain = factory.CreateSwapChain(device, contexts[0], new SwapChainDesc(),
            new FullScreenModeDesc(), WindowHandle.CreateWin32Window(window.Handle));

        var displayMode = factory.EnumerateDisplayModes(new Version(11, 0), 0, 0, TextureFormat.Rgba8Unorm)
            .First();
        swapChain.SetFullscreenMode(displayMode);
        swapChain.Dispose();
        foreach (var ctx in contexts)
            ctx.Dispose();
        device.Dispose();
    }

    [Test]
    public void MustSetWindowedMode()
    {
        using var window = new TestWindow();
        using var factory = DiligentCore.GetEngineFactoryD3D11();
        (var device, var contexts) = factory.CreateDeviceAndContexts(new EngineD3D11CreateInfo());
        var swapChain = factory.CreateSwapChain(device, contexts[0], new SwapChainDesc(),
            new FullScreenModeDesc(), WindowHandle.CreateWin32Window(window.Handle));

        swapChain.SetWindowedMode();

        swapChain.Dispose();
        foreach (var ctx in contexts)
            ctx.Dispose();
        device.Dispose();
    }

    [Test]
    public void MustSetMaximumFrameLatency()
    {
        using var window = new TestWindow();
        using var factory = DiligentCore.GetEngineFactoryD3D11();
        (var device, var contexts) = factory.CreateDeviceAndContexts(new EngineD3D11CreateInfo());
        var swapChain = factory.CreateSwapChain(device, contexts[0], new SwapChainDesc(),
            new FullScreenModeDesc(), WindowHandle.CreateWin32Window(window.Handle));

        swapChain.SetMaximumFrameLatency(30);

        swapChain.Dispose();
        foreach (var ctx in contexts)
            ctx.Dispose();
        device.Dispose();
    }

    [Test]
    public void MustGetSwapChainDesc()
    {
        using var window = new TestWindow();
        using var factory = DiligentCore.GetEngineFactoryD3D11();
        (var device, var contexts) = factory.CreateDeviceAndContexts(new EngineD3D11CreateInfo());
        var swapChain = factory.CreateSwapChain(device, contexts[0], new SwapChainDesc(),
            new FullScreenModeDesc(), WindowHandle.CreateWin32Window(window.Handle));

        var desc = swapChain.Desc;
        var windowSize = window.Size;
        Assert.That(desc.Width, Is.EqualTo(windowSize.Width));
        Assert.That(desc.Height, Is.EqualTo(windowSize.Height));

        swapChain.Dispose();
        foreach (var ctx in contexts)
            ctx.Dispose();
        device.Dispose();
    }

    [Test]
    public void MustGetCurrentBackRTV()
    {
        using var window = new TestWindow();
        using var factory = DiligentCore.GetEngineFactoryD3D11();
        (var device, var contexts) = factory.CreateDeviceAndContexts(new EngineD3D11CreateInfo());
        var swapChain = factory.CreateSwapChain(device, contexts[0], new SwapChainDesc(),
            new FullScreenModeDesc(), WindowHandle.CreateWin32Window(window.Handle));

        Assert.That(swapChain.CurrentBackBufferRTV, Is.Not.Null);

        swapChain.Dispose();
        foreach (var ctx in contexts)
            ctx.Dispose();
        device.Dispose();
    }
    
    [Test]
    public void MustGetDepthBufferDSV()
    {
        using var window = new TestWindow();
        using var factory = DiligentCore.GetEngineFactoryD3D11();
        (var device, var contexts) = factory.CreateDeviceAndContexts(new EngineD3D11CreateInfo());
        var swapChain = factory.CreateSwapChain(device, contexts[0], new SwapChainDesc(),
            new FullScreenModeDesc(), WindowHandle.CreateWin32Window(window.Handle));

        Assert.That(swapChain.DepthBufferDSV, Is.Not.Null);

        swapChain.Dispose();
        foreach (var ctx in contexts)
            ctx.Dispose();
        device.Dispose();
    }
}
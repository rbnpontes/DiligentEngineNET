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
}
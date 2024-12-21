using Diligent.Tests.Utils;

namespace Diligent.Tests;

[TestFixture]
[NonParallelizable]
public class EngineFactoryOpenGlTest
{
    private IEngineFactoryOpenGL GetFactory()
    {
        var factory = DiligentCore.GetEngineFactoryOpenGL() ?? throw new NullReferenceException();
        factory.SetMessageCallback(DebugOutput.MessageCallback);
        return factory;
    }

    [Test]
    [Platform("Win")]
    public void MustCreateDeviceContextAndSwapChain()
    {
        if (SystemUtils.IsRunningAsHeadless())
        {
            Assert.Pass();
            return;
        }
        
        using var window = new TestWindow();
        using var factory = GetFactory();
        
        var createInfo = new EngineOpenGlCreateInfo()
        {
            EnableValidation = true,
            Window = WindowHandle.CreateWin32Window(window.Handle)
        };
        
        var (device, context, swapChain) = factory.CreateDeviceAndSwapChain(createInfo, new SwapChainDesc());
        
        Assert.That(device, Is.Not.Null);
        Assert.That(context, Is.Not.Null);
        Assert.That(swapChain, Is.Not.Null);
        
        swapChain.Dispose();
        context.Dispose();
        device.Dispose();
    }
    
    [Test]
    [Platform("Win")]
    public void MustCreateDeviceAndContextFromActiveGlContext()
    {
        if (SystemUtils.IsRunningAsHeadless())
        {
            Assert.Pass();
            return;
        }
        
        using var window = new TestWindow();
        using var factory = GetFactory();
        
        var createInfo = new EngineOpenGlCreateInfo()
        {
            EnableValidation = true,
            Window = WindowHandle.CreateWin32Window(window.Handle)
        };

        var (device, context) = factory.AttachToActiveGLContext(createInfo);
        Assert.That(device, Is.Not.Null);
        Assert.That(context, Is.Not.Null);
        
        context.Dispose();
        device.Dispose();
    }
}
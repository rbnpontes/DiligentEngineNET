using Diligent.Tests.Utils;

namespace Diligent.Tests;

[TestFixture]
[NonParallelizable]
public class EngineFactoryVkTest
{
    private IEngineFactoryVk GetFactory()
    {
        var factory = DiligentCore.GetEngineFactoryVk() ?? throw new NullReferenceException();
        factory.SetMessageCallback(DebugOutput.MessageCallback);
        return factory;
    }

    [Test]
    public void MustCreateDeviceAndContext()
    {
        using var factory = GetFactory();
        var createInfo = new EngineVkCreateInfo()
        {
            EnableValidation = true,
        };
        (var device, var contexts) = factory.CreateDeviceAndContexts(new EngineVkCreateInfo());
        
        Assert.That(device, Is.Not.Null);
        Assert.That(contexts, Has.Length.GreaterThan(0));

        contexts.First().Flush();
        foreach (var ctx in contexts)
            ctx.Dispose();
        
        Assert.That(device.ReferenceCounters.NumStrongRefs, Is.Not.GreaterThan(1));
        device.Dispose();
    }

    [Test]
    public void MustCreateDeferredContexts()
    {
        using var factory = GetFactory();
        var createInfo = new EngineVkCreateInfo()
        {
            EnableValidation = true,
            NumDeferredContexts = 3
        };
        (var device, var contexts) = factory.CreateDeviceAndContexts(createInfo);
        Assert.That(contexts, Has.Length.EqualTo(4));

		contexts.First().Flush();
        foreach (var ctx in contexts)
            ctx.Dispose();
        
		Assert.That(device.ReferenceCounters.NumStrongRefs, Is.Not.GreaterThan(1));
        device.Dispose();
    }

    [Test]
    [Platform("Win")]
    public void MustCreateSwapChain()
    {
        if (SystemUtils.IsRunningAsHeadless())
        {
            Assert.Pass();
            return;
        }
        
        using var window = new TestWindow();
        using var factory = GetFactory();
        (var device, var contexts) = factory.CreateDeviceAndContexts(new EngineVkCreateInfo());
        var immediateCtx = contexts.First();
        
        var swapChain = factory.CreateSwapChain(device, immediateCtx, new SwapChainDesc(),
            WindowHandle.CreateWin32Window(window.Handle));


		swapChain.Dispose();
        contexts.First().Flush();
        foreach (var ctx in contexts)
            ctx.Dispose();
        Assert.That(device.ReferenceCounters.NumStrongRefs, Is.Not.GreaterThan(1));
        device.Dispose();
    }
}
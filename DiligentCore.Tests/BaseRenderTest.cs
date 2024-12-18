using Diligent.Tests.Utils;

namespace Diligent.Tests;

public class BaseRenderTest
{
    protected IEngineFactory Factory;
    protected IRenderDevice Device;
    protected IDeviceContext Context;
    
    public BaseRenderTest()
    {
        var vkFactory = DiligentCore.GetEngineFactoryVk() ?? throw new NullReferenceException();
        var (device, contexts) = vkFactory.CreateDeviceAndContexts(new EngineVkCreateInfo() { EnableValidation = true });
        Factory = vkFactory;
        Device = device;
        Context = contexts.First();
        Factory.SetMessageCallback(DebugOutput.MessageCallback);
    }

    [OneTimeTearDown]
    public void CleanupResources()
    {
        Context.Flush();
        Context.Dispose();

        if (Device.ReferenceCounters.NumStrongRefs > 1)
            throw new Exception("Device RefCount is greater than 1");
        Device.Dispose();
        Factory.Dispose();
    }
}
using Diligent.Tests.Utils;

namespace Diligent.Tests;

public class BaseRenderTest
{
    protected IEngineFactory Factory;
    protected IRenderDevice Device;
    protected IDeviceContext Context;
    
    public BaseRenderTest()
    {
        // if (OperatingSystem.IsWindows())
        // {
        //     var d3dFactory = DiligentCore.GetEngineFactoryD3D11() ?? throw new NullReferenceException();
        //     var (device, contexts) = d3dFactory.CreateDeviceAndContexts(new EngineD3D11CreateInfo());
        //     Factory = d3dFactory;
        //     Device = device;
        //     Context = contexts.First();
        // }
        // else
        // {
        //     var vkFactory = DiligentCore.GetEngineFactoryVk() ?? throw new NullReferenceException();
        //     var (device, contexts) = vkFactory.CreateDeviceAndContexts(new EngineVkCreateInfo());
        //     Factory = vkFactory;
        //     Device = device;
        //     Context = contexts.First();
        // }
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
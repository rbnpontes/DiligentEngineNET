namespace Diligent.Tests;

public class BaseRenderTest
{
    protected IEngineFactory Factory;
    protected IRenderDevice Device;
    protected IDeviceContext Context;
    
    public BaseRenderTest()
    {
        if (OperatingSystem.IsWindows())
        {
            var d3dFactory = DiligentCore.GetEngineFactoryD3D11() ?? throw new NullReferenceException();
            var (device, contexts) = d3dFactory.CreateDeviceAndContexts(new EngineD3D11CreateInfo());
            Factory = d3dFactory;
            Device = device;
            Context = contexts.First();
        }
        else
        {
            var vkFactory = DiligentCore.GetEngineFactoryVk() ?? throw new NullReferenceException();
            var (device, contexts) = vkFactory.CreateDeviceAndContexts(new EngineVkCreateInfo());
            Factory = vkFactory;
            Device = device;
            Context = contexts.First();
        }
    }

    [OneTimeTearDown]
    public void CleanupResources()
    {
        Context.Dispose();
        Device.Dispose();
        Factory.Dispose();
    }
}
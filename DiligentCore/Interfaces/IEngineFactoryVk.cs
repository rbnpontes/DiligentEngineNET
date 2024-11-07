namespace Diligent;

public interface IEngineFactoryVk : IEngineFactory
{
    (IRenderDevice, IDeviceContext[]) CreateAndContexts(EngineVkCreateInfo createInfo);

    ISwapChain CreateSwapChain(IRenderDevice device, IDeviceContext immediateContext, SwapChainDesc swapChainDesc,
        WindowHandle window);
}
namespace Diligent;

internal partial class EngineFactoryVk : IEngineFactoryVk
{
    internal EngineFactoryVk(IntPtr handle) : base(handle)
    {
    }

    public (IRenderDevice, IDeviceContext[]) CreateAndContexts(EngineVkCreateInfo createInfo)
    {
        throw new NotImplementedException();
    }

    public ISwapChain CreateSwapChain(IRenderDevice device, IDeviceContext immediateContext,
        SwapChainDesc swapChainDesc,
        WindowHandle window)
    {
        throw new NotImplementedException();
    }
}
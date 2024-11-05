namespace Diligent;

public interface IEngineFactoryD3D11 : IEngineFactory
{
    (IRenderDevice, IDeviceContext[]) CreateDeviceAndContexts(EngineD3D11CreateInfo createInfo);

    ISwapChain CreateSwapChain(IRenderDevice device, IDeviceContext immediateContext, SwapChainDesc swapChainDesc,
        FullScreenModeDesc fullScreenDesc, NativeWindow window);

    DisplayModeAttribs[] EnumerateDisplayModes(Version version, uint adapterId, uint outputId, TextureFormat format);
}
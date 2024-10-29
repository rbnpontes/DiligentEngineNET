namespace Diligent;

public interface IEngineFactoryD3D11 : IEngineFactory
{
    (IDeviceContext[], IRenderDevice) CreateDeviceAndContexts(EngineCreateInfo createInfo);

    ISwapChain CreateSwapChain(IRenderDevice device, IDeviceContext immediateContext, SwapChainDesc swapChainDesc,
        FullScreenModeDesc fullScreenDesc, NativeWindow window);

    DisplayModeAttribs[] EnumerateDisplayModes(Version version, uint adapterId, uint outputId, TextureFormat format);
}
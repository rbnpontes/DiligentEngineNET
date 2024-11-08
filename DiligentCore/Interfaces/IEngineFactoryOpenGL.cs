namespace Diligent;

public interface IEngineFactoryOpenGL : IEngineFactory
{
    public (IRenderDevice, IDeviceContext, ISwapChain) CreateDeviceAndSwapChain(EngineOpenGlCreateInfo createInfo, SwapChainDesc swapChainDesc);
    public IHLSL2GLSLConverter CreateHLSL2GLSLConverter();
    public (IRenderDevice, IDeviceContext) AttachToActiveGLContext(EngineOpenGlCreateInfo createInfo);
}
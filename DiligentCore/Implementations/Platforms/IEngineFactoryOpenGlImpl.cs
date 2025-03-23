namespace Diligent.Platforms.Default;

internal interface IEngineFactoryOpenGlImpl
{
    public (IRenderDevice, IDeviceContext, ISwapChain) CreateDeviceAndSwapChain(EngineOpenGlCreateInfo createInfo, SwapChainDesc swapChainDesc);
    public IHLSL2GLSLConverter CreateHlsl2GLSLConverter();
    public (IRenderDevice, IDeviceContext) AttachToActiveGLContext(EngineOpenGlCreateInfo createInfo);
    
}
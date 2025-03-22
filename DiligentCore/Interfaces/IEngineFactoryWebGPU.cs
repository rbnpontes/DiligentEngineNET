namespace Diligent;

public interface IEngineFactoryWebGPU : IEngineFactory
{
    public (IRenderDevice, IDeviceContext) CreateDeviceAndContext(EngineWebGPUCreateInfo createInfo);
    public ISwapChain CreateSwapChain(IRenderDevice device, IDeviceContext immediateContext, SwapChainDesc swapChainDesc, WindowHandle window);
    public (IRenderDevice, IDeviceContext) AttachToWebGPUDevice(IntPtr wgpuInstance, IntPtr wgpuAdapter, IntPtr wgpuDevice, EngineWebGPUCreateInfo createInfo);
    public IntPtr GetProcessTable();
}
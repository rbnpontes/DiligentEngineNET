namespace Diligent;

public interface IEngineFactoryD3D12 : IEngineFactory
{
     void LoadD3D12(string dllName = "d3d12.dll");
     (IRenderDevice, IDeviceContext[]) CreateDeviceAndContext(EngineD3D12CreateInfo createInfo);

     (IMemoryAllocator, ICommandQueueD3D12) CreateCommandQueueD3D12(IntPtr d3d12NativeDevice,
          IntPtr d3d12NativeCommandQueue);

     (IRenderDevice, IDeviceContext[]) AttachToD3D12Device(IntPtr d3d12NativeDevice, ICommandQueueD3D12[] commandQueues,
          EngineD3D12CreateInfo createInfo);
     
     ISwapChain CreateSwapChain(IRenderDevice device, IDeviceContext immediateContext, SwapChainDesc swapChainDec,
          FullScreenModeDesc fullScreenModeDesc, WindowHandle window);

     DisplayModeAttribs[] EnumerateDisplayModes(Version minFeatureLevel, uint adapterId, uint outputId, TextureFormat format);
}
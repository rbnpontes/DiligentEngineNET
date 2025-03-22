using System.Runtime.InteropServices;
using System.Security;
using Diligent.Utils;

namespace Diligent;

internal partial class EngineFactoryWebGPU(IntPtr handle) : EngineFactory(handle), IEngineFactoryWebGPU
{
    partial class Interop
    {
        [LibraryImport(Constants.LibName)]
        [SuppressUnmanagedCodeSecurity]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void engine_factory_web_gpu_create_swap_chain_web_gpu(IntPtr handle, IntPtr renderDevice,
            IntPtr immediateContext, IntPtr swapChainDesc, IntPtr window, IntPtr swapChain);
    }
    
    public unsafe (IRenderDevice, IDeviceContext) CreateDeviceAndContext(EngineWebGPUCreateInfo createInfo)
    {
        var createInfoData = EngineWebGPUCreateInfo.GetInternalStruct(createInfo);
        var renderDevicePtr = IntPtr.Zero;
        var deviceContextPtr = IntPtr.Zero;
        
        Interop.engine_factory_web_gpu_create_device_and_contexts_web_gpu(
            Handle,
            new IntPtr(&createInfoData),
            new IntPtr(&renderDevicePtr),
            new IntPtr(&deviceContextPtr));
        
        return (
            DiligentObjectsFactory.CreateRenderDevice(renderDevicePtr),
            DiligentObjectsFactory.CreateDeviceContext(deviceContextPtr)
        );
    }

    public unsafe ISwapChain CreateSwapChain(IRenderDevice device, IDeviceContext immediateContext, SwapChainDesc swapChainDesc,
        WindowHandle window)
    {
        var swapChainDescData = SwapChainDesc.GetInternalStruct(swapChainDesc);
        var windowData = WindowHandle.GetInternalStruct(window);
        var swapChainPtr = IntPtr.Zero;
        
        Interop.engine_factory_web_gpu_create_swap_chain_web_gpu(Handle,
            device.Handle,
            immediateContext.Handle,
            new IntPtr(&swapChainDescData),
            new IntPtr(&windowData),
            new IntPtr(&swapChainPtr));
        return DiligentObjectsFactory.CreateSwapChain(swapChainPtr);
    }

    public unsafe (IRenderDevice, IDeviceContext) AttachToWebGPUDevice(IntPtr wgpuInstance, IntPtr wgpuAdapter, IntPtr wgpuDevice,
        EngineWebGPUCreateInfo createInfo)
    {
        var createInfoData = EngineWebGPUCreateInfo.GetInternalStruct(createInfo);
        var renderDevicePtr = IntPtr.Zero;
        var deviceContextPtr = IntPtr.Zero;
        
        Interop.engine_factory_web_gpu_attach_to_web_gpudevice(Handle,
            wgpuInstance,
            wgpuAdapter,
            wgpuDevice,
            new IntPtr(&createInfoData),
            new IntPtr(&renderDevicePtr),
            new IntPtr(&deviceContextPtr));

        return (
            DiligentObjectsFactory.CreateRenderDevice(renderDevicePtr),
            DiligentObjectsFactory.CreateDeviceContext(deviceContextPtr)
        );
    }

    public IntPtr GetProcessTable()
    {
        return Interop.engine_factory_web_gpu_get_process_table(Handle);
    }
}
using System.Runtime.InteropServices;
using System.Security;
using Diligent.Utils;

namespace Diligent;

internal partial class EngineFactoryVk : IEngineFactoryVk
{
    partial class Interop
    {
        [LibraryImport(Constants.LibName)]
        [SuppressUnmanagedCodeSecurity]
        [UnmanagedCallConv(CallConvs = new System.Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        public static partial void engine_factory_vk_create_swap_chain_vk(IntPtr handle, IntPtr renderDevice,
            IntPtr immediateContext, IntPtr swapChainDesc, IntPtr window, IntPtr swapChain);
    }
    
    internal EngineFactoryVk(IntPtr handle) : base(handle)
    {
    }

    public unsafe (IRenderDevice, IDeviceContext[]) CreateDeviceAndContexts(EngineVkCreateInfo createInfo)
    {
        var numDeferredContexts = GetNumDeferredContexts(createInfo);
        var createInfoData = EngineVkCreateInfo.GetInternalStruct(createInfo);
        var renderDevicePtr = IntPtr.Zero;
        var deviceContexts = new IntPtr[numDeferredContexts];
        
        fixed(void* deviceContextsPtr = deviceContexts.AsSpan())
            Interop.engine_factory_vk_create_device_and_contexts_vk(
                Handle,
                new IntPtr(&createInfoData),
                new IntPtr(&renderDevicePtr),
                new IntPtr(deviceContextsPtr));

        return (
            DiligentObjectsFactory.CreateRenderDevice(renderDevicePtr),
            DiligentObjectsFactory.CreateDeviceContexts(deviceContexts)
        );
    }

    public unsafe ISwapChain CreateSwapChain(IRenderDevice device, IDeviceContext immediateContext,
        SwapChainDesc swapChainDesc,
        WindowHandle window)
    {
        var swapChainDescData = SwapChainDesc.GetInternalStruct(swapChainDesc);
        var windowData = WindowHandle.GetInternalStruct(window);
        var linuxWindowData = LinuxWindowHandle.GetInternalStruct(window.LinuxWindowHandle);
        var swapChainPtr = IntPtr.Zero;
        
        if(OperatingSystem.IsLinux())
            windowData.window_handle_ = new IntPtr(&linuxWindowData);
        
        Interop.engine_factory_vk_create_swap_chain_vk(Handle, 
            device.Handle, 
            immediateContext.Handle, 
            new IntPtr(&swapChainDescData),
            new IntPtr(&windowData),
            new IntPtr(&swapChainPtr));

        return DiligentObjectsFactory.CreateSwapChain(swapChainPtr);
    }

    public unsafe void EnableDeviceSimulation()
    {
        Interop.engine_factory_vk_enable_device_simulation(Handle);
    }
}
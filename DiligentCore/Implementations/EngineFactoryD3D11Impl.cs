using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Diligent;

internal partial class EngineFactoryD3D11 : IEngineFactoryD3D11
{
    internal partial class Interop
    {
        [LibraryImport(Constants.LibName)]
        [SuppressUnmanagedCodeSecurity]
        [UnmanagedCallConv(CallConvs = new System.Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        public static partial void engine_factory_d3d11_create_swapchain_d3d11(IntPtr handle, IntPtr renderDevice,
            IntPtr immediateContext, IntPtr swapChainDesc, IntPtr fullScreenDesc, IntPtr window, IntPtr swapChain);
    }
    
    internal EngineFactoryD3D11(IntPtr handle) : base(handle)
    {
    }

    public unsafe (IRenderDevice, IDeviceContext[]) CreateDeviceAndContexts(EngineD3D11CreateInfo createInfo)
    {
        var numDevices = (int)(int.Max((int)createInfo.NumImmediateContexts, 1) + createInfo.NumDeferredContexts);
        var createInfoData = EngineD3D11CreateInfo.GetInternalStruct(createInfo);
        var createInfoPtr = &createInfoData;
        var renderDevicePtr = IntPtr.Zero;

        var deviceContextsPointers = new IntPtr[numDevices];
        fixed(void* deviceContextsPtr = deviceContextsPointers.AsSpan())
            Interop.engine_factory_d3d11_create_device_and_contexts_d3d11(
                Handle,
                new IntPtr(createInfoPtr),
                new IntPtr(&renderDevicePtr),
                new IntPtr(deviceContextsPtr));

        return (
            CreateRenderDevice(renderDevicePtr),
            CreateDeviceContexts(deviceContextsPointers)
        );

        IRenderDevice CreateRenderDevice(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                throw new NullReferenceException($"Failed to create {nameof(IRenderDevice)}");
            return NativeObjectRegistry.GetOrCreate(() => new RenderDevice(handle), handle);
        }

        IDeviceContext[] CreateDeviceContexts(IntPtr[] handle)
        {
            return handle.Select(x =>
            {
                return NativeObjectRegistry.GetOrCreate<IDeviceContext>(() => new DeviceContext(x), x);
            }).ToArray();
        }
    }

    public unsafe ISwapChain CreateSwapChain(IRenderDevice device, IDeviceContext immediateContext,
        SwapChainDesc swapChainDesc,
        FullScreenModeDesc fullScreenDesc, WindowHandle window)
    {
        var windowHandleData = WindowHandle.GetInternalStruct(window);
        var swapChainDescData = SwapChainDesc.GetInternalStruct(swapChainDesc);
        var fullScreenDescData = FullScreenModeDesc.GetInternalStruct(fullScreenDesc);
        var swapChainPtr = IntPtr.Zero;
     
        Interop.engine_factory_d3d11_create_swapchain_d3d11(Handle, 
            device.Handle, 
            immediateContext.Handle, 
            new IntPtr(&swapChainDescData),
            new IntPtr(&fullScreenDescData),
            new IntPtr(&windowHandleData),
            new IntPtr(&swapChainPtr));

        return InternalCreateSwapChain(swapChainPtr);
        
        ISwapChain InternalCreateSwapChain(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                throw new NullReferenceException($"Failed to create {nameof(ISwapChain)}");

            return NativeObjectRegistry.GetOrCreate(() => new SwapChain(handle), handle);
        }
    }

    public unsafe DisplayModeAttribs[] EnumerateDisplayModes(Version minFeatureLevel, uint adapterId, uint outputId,
        TextureFormat format)
    {
        uint numDisplayModes = 0;
        var minFeatureLvlStruct = Version.GetInternalStruct(minFeatureLevel);

        var versionPtr = &minFeatureLvlStruct;
        var numDisplayModesPtr = &numDisplayModes;

        Interop.engine_factory_d3d11_enumerate_display_modes(
            Handle,
            new IntPtr(versionPtr),
            adapterId,
            outputId,
            format,
            new IntPtr(numDisplayModesPtr),
            IntPtr.Zero);

        if (numDisplayModes == 0)
            return [];

        var displayModes = new DisplayModeAttribs.__Internal[numDisplayModes];
        fixed (DisplayModeAttribs.__Internal* displayModesPtr = displayModes)
            Interop.engine_factory_d3d11_enumerate_display_modes(
                Handle,
                new IntPtr(versionPtr),
                adapterId,
                outputId,
                format,
                new IntPtr(numDisplayModesPtr),
                new IntPtr(displayModesPtr));

        return displayModes.Select(DisplayModeAttribs.FromInternalStruct).ToArray();
    }
}
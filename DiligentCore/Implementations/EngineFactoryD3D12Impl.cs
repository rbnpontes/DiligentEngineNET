using System.Runtime.InteropServices;
using System.Security;
using Diligent.Utils;

namespace Diligent;

internal partial class EngineFactoryD3D12 : IEngineFactoryD3D12
{
    partial class Interop
    {
        [LibraryImport(Constants.LibName)]
        [SuppressUnmanagedCodeSecurity]
        [UnmanagedCallConv(CallConvs = new System.Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        public static partial void engine_factory_d3d12_create_swap_chain_d3d12(IntPtr factory, IntPtr device,
            IntPtr immediateContext, IntPtr swapChainDesc, IntPtr fullScreenModeDesc, IntPtr window, IntPtr swapChain);
    }

    internal EngineFactoryD3D12(IntPtr handle) : base(handle)
    {
    }

    public void LoadD3D12(string dllName = "d3d12.dll")
    {
        using var strAlloc = new StringAllocator();
        Interop.engine_factory_d3d12_load_d3d12(Handle, strAlloc.Acquire(dllName));
    }

    public unsafe (IRenderDevice, IDeviceContext[]) CreateDeviceAndContext(EngineD3D12CreateInfo createInfo)
    {
        using var strAlloc = new StringAllocator();
        var numDeferredContexts = GetNumDeferredContexts(createInfo);

        var createInfoData = EngineD3D12CreateInfo.GetInternalStruct(createInfo);
        var renderDevicePtr = IntPtr.Zero;
        var deviceContexts = new IntPtr[numDeferredContexts];

        createInfoData.D3D12DllName = strAlloc.Acquire(createInfo.D3D12DllName);
        createInfoData.pDxCompilerPath = strAlloc.Acquire(createInfo.DxCompilerPath);
        fixed (void* deviceContextsPtr = deviceContexts.AsSpan())
            Interop.engine_factory_d3d12_create_device_and_contexts_d3d12(Handle,
                new IntPtr(&createInfoData),
                new IntPtr(&renderDevicePtr),
                new IntPtr(deviceContextsPtr));
        return (
            DiligentObjectsFactory.CreateRenderDevice(renderDevicePtr),
            DiligentObjectsFactory.CreateDeviceContexts(deviceContexts)
        );
    }

    public (IMemoryAllocator, ICommandQueueD3D12) CreateCommandQueueD3D12(IntPtr d3d12NativeDevice, IntPtr d3d12NativeCommandQueue)
    {
        throw new NotImplementedException();
    }

    public unsafe (IRenderDevice, IDeviceContext[]) AttachToD3D12Device(IntPtr d3d12NativeDevice,
        ICommandQueueD3D12[] commandQueues,
        EngineD3D12CreateInfo createInfo)
    {
        using var strAlloc = new StringAllocator();
        var numDeferredContexts = GetNumDeferredContexts(createInfo);
        
        var createInfoData = EngineD3D12CreateInfo.GetInternalStruct(createInfo);
        var renderDevicePtr = IntPtr.Zero;
        var deviceContexts = new IntPtr[numDeferredContexts];
        var commandQueuesPointers = commandQueues
            .Select(x => x.Handle)
            .ToArray();
        
        createInfoData.D3D12DllName = strAlloc.Acquire(createInfo.D3D12DllName);
        createInfoData.pDxCompilerPath = strAlloc.Acquire(createInfo.DxCompilerPath);
        fixed (void* deviceContextsPtr = deviceContexts.AsSpan())
        {
            fixed (void* commandQueuesPtr = commandQueuesPointers.AsSpan())
            {
                Interop.engine_factory_d3d12_attach_to_d3d12device(Handle,
                    d3d12NativeDevice,
                    (uint)commandQueuesPointers.Length,
                    new IntPtr(commandQueuesPtr),
                    new IntPtr(&createInfoData),
                    new IntPtr(&renderDevicePtr),
                    new IntPtr(deviceContextsPtr));
                
            }
        }

        return (
            DiligentObjectsFactory.CreateRenderDevice(renderDevicePtr),
            DiligentObjectsFactory.CreateDeviceContexts(deviceContexts)
        );
    }

    public unsafe ISwapChain CreateSwapChain(IRenderDevice device, IDeviceContext immediateContext, SwapChainDesc swapChainDec,
        FullScreenModeDesc fullScreenModeDesc, WindowHandle window)
    {
        var swapChainDescData = SwapChainDesc.GetInternalStruct(swapChainDec);
        var fullScreenModeDescData = FullScreenModeDesc.GetInternalStruct(fullScreenModeDesc);
        var windowData = WindowHandle.GetInternalStruct(window);
        var swapChain = IntPtr.Zero;
        
        Interop.engine_factory_d3d12_create_swap_chain_d3d12(Handle,
            device.Handle,
            immediateContext.Handle,
            new IntPtr(&swapChainDescData),
            new IntPtr(&fullScreenModeDescData),
            new IntPtr(&windowData),
            new IntPtr(&swapChain));

        return DiligentObjectsFactory.CreateSwapChain(swapChain);
    }

    public unsafe DisplayModeAttribs[] EnumerateDisplayModes(Version minFeatureLevel, uint adapterId, uint outputId,
        TextureFormat format)
    {
        uint numDisplayModes = 0;
        var minFeatureLvlData = Version.GetInternalStruct(minFeatureLevel);
        
        Interop.engine_factory_d3d12_enumerate_display_modes(Handle,
            new IntPtr(&minFeatureLvlData),
            adapterId,
            outputId,
            format,
            new IntPtr(&numDisplayModes),
            IntPtr.Zero);

        if (numDisplayModes == 0)
            return [];

        var displayModes = new DisplayModeAttribs.__Internal[numDisplayModes];
        fixed(void* displayModesPtr = displayModes)
            Interop.engine_factory_d3d12_enumerate_display_modes(Handle,
                new IntPtr(&minFeatureLvlData),
                adapterId,
                outputId,
                format,
                new IntPtr(&numDisplayModes),
                new IntPtr(displayModesPtr));
        return displayModes.Select(DisplayModeAttribs.FromInternalStruct).ToArray();
    }
}
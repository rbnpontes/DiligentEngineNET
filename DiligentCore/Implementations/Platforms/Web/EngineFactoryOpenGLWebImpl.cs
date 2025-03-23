using System.Runtime.CompilerServices;
using Diligent.Utils;

namespace Diligent.Platforms.Default.Web;

internal class EngineFactoryOpenGLWebImpl(IntPtr handle) : IEngineFactoryOpenGlImpl
{
    private unsafe (IntPtr, IntPtr) BuildCreateInfoAndSwapChainDescData(EngineOpenGlCreateInfo createInfo, SwapChainDesc swapChainDesc)
    {
        var requiredSize = Unsafe.SizeOf<EngineOpenGlCreateInfo.__Internal>()
                           + Unsafe.SizeOf<WindowHandle.__Internal>()
                           + Unsafe.SizeOf<SwapChainDesc.__Internal>()
                           + Unsafe.SizeOf<OpenXRAttribs.__Internal>();
        var createInfoPtr = WebScratchBuffer.Require(requiredSize);
        var windowHandlePtr = createInfoPtr + Unsafe.SizeOf<EngineOpenGlCreateInfo.__Internal>();
        var swapChainDescPtr = windowHandlePtr + Unsafe.SizeOf<WindowHandle.__Internal>();
        var openXrAttribsPtr = swapChainDescPtr + Unsafe.SizeOf<SwapChainDesc.__Internal>();
        
        var createInfoData = EngineOpenGlCreateInfo.GetInternalStruct(createInfo);
        var windowHandleData = WindowHandle.GetInternalStruct(createInfo.Window ?? WindowHandleFactory.CreateNull());
        var openXrAttribsData = OpenXRAttribs.GetInternalStruct(createInfo.XRAttribs ?? new OpenXRAttribs());
        var swapChainDescData = SwapChainDesc.GetInternalStruct(swapChainDesc);
        
        createInfoData.Window = windowHandlePtr;
        createInfoData.pXRAttribs = createInfo.XRAttribs is not null 
            ? openXrAttribsPtr
            : IntPtr.Zero;
        
        var buffer = stackalloc byte[requiredSize];
        
        *((EngineOpenGlCreateInfo.__Internal*)buffer) = createInfoData;
        buffer += Unsafe.SizeOf<EngineOpenGlCreateInfo.__Internal>();
        
        *((WindowHandle.__Internal*)buffer) = windowHandleData;
        buffer += Unsafe.SizeOf<WindowHandle.__Internal>();
        
        *((SwapChainDesc.__Internal*)buffer) = swapChainDescData;
        buffer += Unsafe.SizeOf<SwapChainDesc.__Internal>();
        
        *((OpenXRAttribs.__Internal*)buffer) = openXrAttribsData;
        buffer += Unsafe.SizeOf<OpenXRAttribs.__Internal>();
        
        // reset buffer offsets
        buffer = buffer - requiredSize;
        
        WebInteropUtils.CopyToDiligent(createInfoPtr, buffer, (uint)requiredSize);
        return (createInfoPtr, swapChainDescPtr);
    }
    
    public unsafe (IRenderDevice, IDeviceContext, ISwapChain) CreateDeviceAndSwapChain(EngineOpenGlCreateInfo createInfo,
        SwapChainDesc swapChainDesc)
    {
        var (createInfoPtr, swapChainDescPtr) = BuildCreateInfoAndSwapChainDescData(createInfo, swapChainDesc);
        using var outputArgs = new WebMemory(sizeof(nint) * 3);
        
        var renderDevicePtr = outputArgs.Handle;
        var deviceCtxPtr = renderDevicePtr + sizeof(nint);
        var swapChainPtr = deviceCtxPtr + sizeof(nint);
        
        EngineFactoryOpenGL.WebInterop.engine_factory_open_gl_create_device_and_swap_chain_gl(
            handle,
            createInfoPtr,
            renderDevicePtr,
            deviceCtxPtr,
            swapChainDescPtr,
            swapChainPtr
        );

        var result = stackalloc uint[3];
        WebInteropUtils.CopyToManaged(outputArgs.Handle, result, outputArgs.Size);

        return (
            DiligentObjectsFactory.CreateRenderDevice((nint)result[0]),
            DiligentObjectsFactory.CreateDeviceContext((nint)result[1]),
            DiligentObjectsFactory.CreateSwapChain((nint)result[2])
        );
    }

    public IHLSL2GLSLConverter CreateHlsl2GLSLConverter()
    {
        throw new NotImplementedException();
    }

    public (IRenderDevice, IDeviceContext) AttachToActiveGLContext(EngineOpenGlCreateInfo createInfo)
    {
        throw new NotImplementedException();
    }
}
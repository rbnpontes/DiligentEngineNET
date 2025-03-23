using Diligent.Utils;

namespace Diligent.Platforms.Default;

internal unsafe class EngineFactoryOpenGLDefaultImpl(IntPtr handle) : IEngineFactoryOpenGlImpl
{
    public (IRenderDevice, IDeviceContext, ISwapChain) CreateDeviceAndSwapChain(EngineOpenGlCreateInfo createInfo,
        SwapChainDesc swapChainDesc)
    {
        var window = createInfo.Window ?? WindowHandleFactory.CreateNull();
        var windowData = WindowHandle.GetInternalStruct(window);
        var linuxWindowData = LinuxWindowHandle.GetInternalStruct(window.LinuxWindowHandle);
        if (OperatingSystem.IsLinux())
            windowData.window_handle_ = new IntPtr(&linuxWindowData);

        var createInfoData = EngineOpenGlCreateInfo.GetInternalStruct(createInfo);
        var openXrAttribsData = OpenXRAttribs.GetInternalStruct(createInfo.XRAttribs ?? new OpenXRAttribs());
        createInfoData.Window = new IntPtr(&windowData);
        if (createInfo.XRAttribs is not null)
            createInfoData.pXRAttribs = new IntPtr(&openXrAttribsData);

        var swapChainData = SwapChainDesc.GetInternalStruct(swapChainDesc);

        var renderDevicePtr = IntPtr.Zero;
        var deviceContextPtr = IntPtr.Zero;
        var swapChainPtr = IntPtr.Zero;

        EngineFactoryOpenGL.Interop.engine_factory_open_gl_create_device_and_swap_chain_gl(handle,
            new IntPtr(&createInfoData),
            new IntPtr(&renderDevicePtr),
            new IntPtr(&deviceContextPtr),
            new IntPtr(&swapChainData),
            new IntPtr(&swapChainPtr)
        );

        return (
            DiligentObjectsFactory.CreateRenderDevice(renderDevicePtr),
            DiligentObjectsFactory.CreateDeviceContext(deviceContextPtr),
            DiligentObjectsFactory.CreateSwapChain(swapChainPtr)
        );
    }

    public IHLSL2GLSLConverter CreateHlsl2GLSLConverter()
    {
        var converterPtr = IntPtr.Zero;
        EngineFactoryOpenGL.Interop.engine_factory_open_gl_create_hlsl2glslconverter(handle, new IntPtr(&converterPtr));
        return DiligentObjectsFactory.CreateHlsl2GlslConverter(converterPtr);
    }

    public (IRenderDevice, IDeviceContext) AttachToActiveGLContext(EngineOpenGlCreateInfo createInfo)
    {
        var window = createInfo.Window ?? WindowHandleFactory.CreateNull();
        var windowData = WindowHandle.GetInternalStruct(window);
        var linuxWindowData = LinuxWindowHandle.GetInternalStruct(window.LinuxWindowHandle);
        if (OperatingSystem.IsLinux())
            windowData.window_handle_ = new IntPtr(&linuxWindowData);

        var createInfoData = EngineOpenGlCreateInfo.GetInternalStruct(createInfo);
        createInfoData.Window = new IntPtr(&windowData);

        var renderDevicePtr = IntPtr.Zero;
        var deviceContextPtr = IntPtr.Zero;
        EngineFactoryOpenGL.Interop.engine_factory_open_gl_attach_to_active_glcontext(handle,
            new IntPtr(&createInfoData),
            new IntPtr(&renderDevicePtr),
            new IntPtr(&deviceContextPtr)
        );

        return (
            DiligentObjectsFactory.CreateRenderDevice(renderDevicePtr),
            DiligentObjectsFactory.CreateDeviceContext(deviceContextPtr)
        );
    }
}
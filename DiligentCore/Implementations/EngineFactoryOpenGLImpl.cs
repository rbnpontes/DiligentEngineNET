using System.Runtime.InteropServices;
using System.Security;
using Diligent.Utils;

namespace Diligent;

internal partial class EngineFactoryOpenGL : IEngineFactoryOpenGL
{
    partial class Interop
    {
        [LibraryImport(Constants.LibName)]
        [SuppressUnmanagedCodeSecurity]
        [UnmanagedCallConv(CallConvs = new System.Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        public static partial void engine_factory_open_gl_create_device_and_swap_chain_gl(IntPtr handle,
            IntPtr createInfo, IntPtr device, IntPtr immediateContext, IntPtr swapChainDesc, IntPtr swapChain);

        [LibraryImport(Constants.LibName)]
        [SuppressUnmanagedCodeSecurity]
        [UnmanagedCallConv(CallConvs = new System.Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        public static partial void engine_factory_open_gl_attach_to_active_glcontext(IntPtr _this, IntPtr arg0,
            IntPtr arg1, IntPtr arg2);
    }

    internal EngineFactoryOpenGL(IntPtr handle) : base(handle)
    {
    }

    public unsafe (IRenderDevice, IDeviceContext, ISwapChain) CreateDeviceAndSwapChain(
        EngineOpenGlCreateInfo createInfo, SwapChainDesc swapChainDesc)
    {
        var window = createInfo.Window ?? WindowHandle.CreateNull();
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

        Interop.engine_factory_open_gl_create_device_and_swap_chain_gl(Handle,
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

    public unsafe IHLSL2GLSLConverter CreateHLSL2GLSLConverter()
    {
        var converterPtr = IntPtr.Zero;
        Interop.engine_factory_open_gl_create_hlsl2glslconverter(Handle, new IntPtr(&converterPtr));
        return DiligentObjectsFactory.CreateHlsl2GlslConverter(converterPtr);
    }

    public unsafe (IRenderDevice, IDeviceContext) AttachToActiveGLContext(EngineOpenGlCreateInfo createInfo)
    {
        var window = createInfo.Window ?? WindowHandle.CreateNull();
        var windowData = WindowHandle.GetInternalStruct(window);
        var linuxWindowData = LinuxWindowHandle.GetInternalStruct(window.LinuxWindowHandle);
        if (OperatingSystem.IsLinux())
            windowData.window_handle_ = new IntPtr(&linuxWindowData);

        var createInfoData = EngineOpenGlCreateInfo.GetInternalStruct(createInfo);
        createInfoData.Window = new IntPtr(&windowData);

        var renderDevicePtr = IntPtr.Zero;
        var deviceContextPtr = IntPtr.Zero;
        Interop.engine_factory_open_gl_attach_to_active_glcontext(Handle,
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
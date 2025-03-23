using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using System.Security;
using Diligent.Platforms.Default;
using Diligent.Platforms.Default.Web;
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

    static partial class WebInterop
    {
        [JSImport("_engine_factory_open_gl_create_device_and_swap_chain_gl", Constants.WebLibName)]
        public static partial void engine_factory_open_gl_create_device_and_swap_chain_gl(IntPtr handle,
            IntPtr createInfo, IntPtr device, IntPtr immediateContext, IntPtr swapChainDesc, IntPtr swapChain);
        [JSImport("_engine_factory_open_gl_attach_to_active_glcontext", Constants.WebLibName)]
        public static partial void engine_factory_open_gl_attach_to_active_glcontext(IntPtr _this, IntPtr arg0,
            IntPtr arg1, IntPtr arg2);
    }

    private IEngineFactoryOpenGlImpl _impl;

    internal EngineFactoryOpenGL(IntPtr handle) : base(handle)
    {
        if (PlatformUtils.IsWasm)
            _impl = new EngineFactoryOpenGLWebImpl(handle);
        else
            _impl = new EngineFactoryOpenGLDefaultImpl(handle);
    }

    public (IRenderDevice, IDeviceContext, ISwapChain) CreateDeviceAndSwapChain(
        EngineOpenGlCreateInfo createInfo, SwapChainDesc swapChainDesc) =>
        _impl.CreateDeviceAndSwapChain(createInfo, swapChainDesc);

    public IHLSL2GLSLConverter CreateHLSL2GLSLConverter() => _impl.CreateHlsl2GLSLConverter();

    public (IRenderDevice, IDeviceContext) AttachToActiveGLContext(EngineOpenGlCreateInfo createInfo) =>
        _impl.AttachToActiveGLContext(createInfo);
}
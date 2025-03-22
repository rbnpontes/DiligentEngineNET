using Diligent.Utils;

namespace Diligent;

internal class WebFactoryBuilder : IFactoryBuilder
{
    public WebFactoryBuilder()
    {
        if(!PlatformUtils.IsWasm)
            throw new PlatformNotSupportedException("WebFactoryBuilder is not supported on this platform");
    }

    public IEngineFactoryD3D11? GetEngineFactoryD3D11() => null;

    public IEngineFactoryD3D12? GetEngineFactoryD3D12() => null;

    public IEngineFactoryVk? GetEngineFactoryVk() => null;

    public IEngineFactoryOpenGL? GetEngineFactoryOpenGL()
    {
        var ptr = DiligentCore.WebInterop.diligent_core_get_opengl_factory();
        if(ptr == IntPtr.Zero)
            return null;
        if(NativeObjectRegistry.TryGetObject(ptr, out var obj))
            return obj as IEngineFactoryOpenGL;
        return new EngineFactoryOpenGL(ptr);
    }

    public IEngineFactoryWebGPU? GetEngineFactoryWebGPU()
    {
        var ptr = DiligentCore.WebInterop.diligent_core_get_webgpu_factory();
        if(ptr == IntPtr.Zero)
            return null;
        if(NativeObjectRegistry.TryGetObject(ptr, out var obj))
            return obj as IEngineFactoryWebGPU;
        return new EngineFactoryWebGPU(ptr);
    }
}
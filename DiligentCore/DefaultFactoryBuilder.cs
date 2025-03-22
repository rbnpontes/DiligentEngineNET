using Diligent.Utils;

namespace Diligent;

internal class DefaultFactoryBuilder : IFactoryBuilder
{
    public DefaultFactoryBuilder()
    {
        if(PlatformUtils.IsWasm)
            throw new PlatformNotSupportedException("DefaultFactoryBuilder is not supported on Wasm");
    }
    
    public IEngineFactoryD3D11? GetEngineFactoryD3D11()
    {
        var ptr = DiligentCore.Interop.diligent_core_get_d3d11_factory();
        if (ptr == IntPtr.Zero)
            return null;
        if (NativeObjectRegistry.TryGetObject(ptr, out var output))
            return output as IEngineFactoryD3D11;
        return new EngineFactoryD3D11(ptr);
    }

    public IEngineFactoryD3D12? GetEngineFactoryD3D12()
    {
        var ptr = DiligentCore.Interop.diligent_core_get_d3d12_factory();
        if (ptr == IntPtr.Zero)
            return null;
        if (NativeObjectRegistry.TryGetObject(ptr, out var output))
            return output as IEngineFactoryD3D12;
        return new EngineFactoryD3D12(ptr);
    }

    public IEngineFactoryVk? GetEngineFactoryVk()
    {
        var ptr = DiligentCore.Interop.diligent_core_get_vk_factory();
        if (ptr == IntPtr.Zero)
            return null;
        if (NativeObjectRegistry.TryGetObject(ptr, out var output))
            return output as IEngineFactoryVk;
        return new EngineFactoryVk(ptr);
    }

    public IEngineFactoryOpenGL? GetEngineFactoryOpenGL()
    {
        var ptr = DiligentCore.Interop.diligent_core_get_opengl_factory();
        if (ptr == IntPtr.Zero)
            return null;
        if (NativeObjectRegistry.TryGetObject(ptr, out var output))
            return output as IEngineFactoryOpenGL;
        return new EngineFactoryOpenGL(ptr);
    }

    public IEngineFactoryWebGPU? GetEngineFactoryWebGPU()
    {
        var ptr = DiligentCore.Interop.diligent_core_get_webgpu_factory();
        if (ptr == IntPtr.Zero)
            return null;
        if (NativeObjectRegistry.TryGetObject(ptr, out var output))
            return output as IEngineFactoryWebGPU;
        return new EngineFactoryWebGPU(ptr);
    }
}
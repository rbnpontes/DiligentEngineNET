namespace Diligent.Utils;

internal static class DiligentObjectsFactory
{
    private static void ThrowIfNullPointer(IntPtr handle, string typeName)
    {
        if (handle == IntPtr.Zero)
            throw new NullReferenceException($"Failed to create {typeName}");
    }
    
    public static IRenderDevice CreateRenderDevice(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IRenderDevice));
        return NativeObjectRegistry.GetOrCreate(() => new RenderDevice(handle), handle);
    }

    public static IDeviceContext CreateDeviceContext(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IDeviceContext));
        return NativeObjectRegistry.GetOrCreate(() => new DeviceContext(handle), handle);
    }
    
    public static IDeviceContext[] CreateDeviceContexts(IntPtr[] handle)
    {
        return handle.Select(x =>
        {
            return NativeObjectRegistry.GetOrCreate<IDeviceContext>(() => new DeviceContext(x), x);
        }).ToArray();
    }

    public static ISwapChain CreateSwapChain(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(ISwapChain));
        return NativeObjectRegistry.GetOrCreate(() => new SwapChain(handle), handle);
    }

    public static IDataBlob CreateDataBlob(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IDataBlob));
        return NativeObjectRegistry.GetOrCreate(() => new DataBlob(handle), handle);
    }

    public static IDearchiver CreateDearchiver(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IDearchiver));
        return NativeObjectRegistry.GetOrCreate(() => new Dearchiver(handle), handle);
    }

    public static IShaderSourceInputStreamFactory CreateShaderSourceInputStreamFactory(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IShaderSourceInputStreamFactory));
        return NativeObjectRegistry.GetOrCreate(() => new ShaderSourceInputStreamFactory(handle), handle);
    }

    public static IFileStream CreateInputStream(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IFileStream));
        return NativeObjectRegistry.GetOrCreate(() => new FileStream(handle), handle);
    }
    
    public static IBuffer CreateBuffer(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IBuffer));
        return NativeObjectRegistry.GetOrCreate(() => new Buffer(handle), handle);
    }

    public static IShader CreateShader(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IShader));
        return NativeObjectRegistry.GetOrCreate(() => new Shader(handle), handle);
    }
    
    public static IHLSL2GLSLConverter CreateHlsl2GlslConverter(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IHLSL2GLSLConverter));
        return NativeObjectRegistry.GetOrCreate(() => new HLSL2GLSLConverter(handle), handle);
    }

    public static IThreadPool CreateThreadPool(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IThreadPool));
        return NativeObjectRegistry.GetOrCreate(() => new ThreadPool(handle), handle);
    }

    public static ITexture CreateTexture(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(ITexture));
        return NativeObjectRegistry.GetOrCreate(() => new Texture(handle), handle);
    }

    public static ISampler CreateSampler(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(ISampler));
        return NativeObjectRegistry.GetOrCreate(() => new Sampler(handle), handle);
    }

    public static IResourceMapping CreateResourceMapping(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IResourceMapping));
        return NativeObjectRegistry.GetOrCreate(() => new ResourceMapping(handle), handle);
    }
}
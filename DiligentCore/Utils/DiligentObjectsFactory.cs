namespace Diligent.Utils;

public static class DiligentObjectsFactory
{
    private static void ThrowIfNullPointer(IntPtr handle, string typeName)
    {
        if (handle == IntPtr.Zero)
            throw new NullReferenceException($"Failed to create {typeName}");
    }
    
    public static IRenderDevice CreateRenderDevice(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IRenderDevice));
        return NativeObjectRegistry.GetOrCreate<IRenderDevice>(() => new RenderDevice(handle), handle);
    }

    public static IDeviceContext CreateDeviceContext(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IDeviceContext));
        return NativeObjectRegistry.GetOrCreate<IDeviceContext>(() => new DeviceContext(handle), handle);
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
        return NativeObjectRegistry.GetOrCreate<ISwapChain>(() => new SwapChain(handle), handle);
    }

    public static IDataBlob CreateDataBlob(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IDataBlob));
        return NativeObjectRegistry.GetOrCreate<IDataBlob>(() => new DataBlob(handle), handle);
    }

    public static IDearchiver CreateDearchiver(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IDearchiver));
        return NativeObjectRegistry.GetOrCreate<IDearchiver>(() => new Dearchiver(handle), handle);
    }

    public static IShaderSourceInputStreamFactory CreateShaderSourceInputStreamFactory(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IShaderSourceInputStreamFactory));
        return NativeObjectRegistry.GetOrCreate<IShaderSourceInputStreamFactory>(() => new ShaderSourceInputStreamFactory(handle), handle);
    }

    public static IFileStream CreateInputStream(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IFileStream));
        return NativeObjectRegistry.GetOrCreate<IFileStream>(() => new FileStream(handle), handle);
    }
    
    public static IBuffer CreateBuffer(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IBuffer));
        return NativeObjectRegistry.GetOrCreate<IBuffer>(() => new Buffer(handle), handle);
    }

    public static IBufferView CreateBufferView(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IBufferView));
        return NativeObjectRegistry.GetOrCreate<IBufferView>(() => new BufferView(handle), handle);
    }

    public static IShader CreateShader(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IShader));
        return NativeObjectRegistry.GetOrCreate<IShader>(() => new Shader(handle), handle);
    }
    
    public static IHLSL2GLSLConverter CreateHlsl2GlslConverter(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IHLSL2GLSLConverter));
        return NativeObjectRegistry.GetOrCreate<IHLSL2GLSLConverter>(() => new HLSL2GLSLConverter(handle), handle);
    }

    public static IThreadPool CreateThreadPool(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IThreadPool));
        return NativeObjectRegistry.GetOrCreate<IThreadPool>(() => new ThreadPool(handle), handle);
    }

    public static ITexture CreateTexture(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(ITexture));
        return NativeObjectRegistry.GetOrCreate<ITexture>(() => new Texture(handle), handle);
    }

    public static ITextureView CreateTextureView(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(ITextureView));
        return NativeObjectRegistry.GetOrCreate<ITextureView>(() => new TextureView(handle), handle);
    }

    public static ISampler CreateSampler(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(ISampler));
        return NativeObjectRegistry.GetOrCreate<ISampler>(() => new Sampler(handle), handle);
    }

    public static IResourceMapping CreateResourceMapping(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IResourceMapping));
        return NativeObjectRegistry.GetOrCreate<IResourceMapping>(() => new ResourceMapping(handle), handle);
    }

    public static IPipelineState CreatePipelineState(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IPipelineState));
        return NativeObjectRegistry.GetOrCreate<IPipelineState>(() => new PipelineState(handle), handle);
    }

    public static IFence CreateFence(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IFence));
        return NativeObjectRegistry.GetOrCreate<IFence>(() => new Fence(handle), handle);
    }

    public static IQuery CreateQuery(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IQuery));
        return NativeObjectRegistry.GetOrCreate<IQuery>(() => new Query(handle), handle);
    }

    public static IRenderPass CreateRenderPass(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IQuery));
        return NativeObjectRegistry.GetOrCreate<IRenderPass>(() => new RenderPass(handle), handle);
    }

    public static IFramebuffer CreateFramebuffer(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IFramebuffer));
        return NativeObjectRegistry.GetOrCreate<IFramebuffer>(() => new Framebuffer(handle), handle);
    }

    public static IBottomLevelAS CreateBLAS(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IBottomLevelAS));
        return NativeObjectRegistry.GetOrCreate<IBottomLevelAS>(() => new BottomLevelAS(handle), handle);
    }

    public static ITopLevelAS CreateTLAS(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(ITopLevelAS));
        return NativeObjectRegistry.GetOrCreate<ITopLevelAS>(() => new TopLevelAS(handle), handle);
    }

    public static IShaderBindingTable CreateSBT(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IShaderBindingTable));
        return NativeObjectRegistry.GetOrCreate<IShaderBindingTable>(() => new ShaderBindingTable(handle), handle);
    }

    public static IPipelineResourceSignature CreatePipelineResourceSignature(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IPipelineResourceSignature));
        return NativeObjectRegistry.GetOrCreate<IPipelineResourceSignature>(() => new PipelineResourceSignature(handle), handle);
    }

    public static IDeviceMemory CreateDeviceMemory(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IDeviceMemory));
        return NativeObjectRegistry.GetOrCreate<IDeviceMemory>(() => new DeviceMemory(handle), handle);
    }

    public static IPipelineStateCache CreatePipelineStateCache(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IDeviceMemory));
        return NativeObjectRegistry.GetOrCreate<IPipelineStateCache>(() => new PipelineStateCache(handle), handle);
    }

    public static IReferenceCounters CreateReferenceCounters(IntPtr handle, INativeObject owner)
    {
        ThrowIfNullPointer(handle, nameof(ReferenceCounters));
        return NativeObjectRegistry.GetOrCreate(() => new ReferenceCounters(handle, owner), handle);
    }

    public static IShaderResourceVariable CreateShaderResourceVariable(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IShaderResourceVariable));
        return NativeObjectRegistry.GetOrCreate<IShaderResourceVariable>(() => new ShaderResourceVariable(handle), handle);
    }

    public static IShaderResourceBinding CreateShaderResourceBinding(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(IShaderResourceBinding));
        return NativeObjectRegistry.GetOrCreate<IShaderResourceBinding>(() => new ShaderResourceBinding(handle), handle);
    }

    public static ICommandList CreateCommandList(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(ICommandList));
        return NativeObjectRegistry.GetOrCreate<ICommandList>(() => new CommandList(handle), handle);
    }

    public static ICommandQueue CreateCommandQueue(IntPtr handle)
    {
        ThrowIfNullPointer(handle, nameof(ICommandQueue));
        return NativeObjectRegistry.GetOrCreate<ICommandQueue>(() => new CommandQueue(handle), handle);
    }
    
    public static IDiligentObject? TryGetOrCreateObject(IntPtr handle)
    {
        if (handle == IntPtr.Zero)
            return null;
            
        NativeObjectRegistry.TryGetObject<IDiligentObject>(handle, out var result);
        return result ?? new UnknownObject(handle);
    }

    public static IDeviceObject? TryGetOrCreateDeviceObject(IntPtr handle)
    {
        if (handle == IntPtr.Zero)
            return null;

        NativeObjectRegistry.TryGetObject<IDeviceObject>(handle, out var result);
        return result ?? new UnknownDeviceObject(handle);
    }
}
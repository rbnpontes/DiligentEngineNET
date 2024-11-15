using Diligent.Utils;

namespace Diligent;

internal unsafe partial class RenderDevice : IRenderDevice
{
    public IEngineFactory EngineFactory
    {
        get
        {
            var factoryPtr = Interop.render_device_get_engine_factory(Handle);
            NativeObjectRegistry.TryGetObject(factoryPtr, out var factory);
            return (IEngineFactory?)factory ?? throw new NullReferenceException();
        }
    }

    public RenderDeviceInfo DeviceInfo
    {
        get
        {
            var renderDeviceInfoPtr = (RenderDeviceInfo.__Internal*)Interop.render_device_get_device_info(Handle);
            return RenderDeviceInfo.FromInternalStruct(*renderDeviceInfoPtr);
        }
    }

    public GraphicsAdapterInfo AdapterInfo
    {
        get
        {
            var adapterInfoPtr = (GraphicsAdapterInfo.__Internal*)Interop.render_device_get_adapter_info(Handle);
            return GraphicsAdapterInfo.FromInternalStruct(*adapterInfoPtr);
        }
    }

    public IThreadPool ShaderCompilationThreadPool => DiligentObjectsFactory.CreateThreadPool(
        Interop.render_device_get_shader_compilation_thread_pool(Handle)
    );

    internal RenderDevice(IntPtr handle) : base(handle)
    {
    }

    public IBuffer CreateBuffer(BufferDesc bufferDesc)
    {
        using var strAlloc = new StringAllocator();

        var bufferDescData = BufferDesc.GetInternalStruct(bufferDesc);
        var bufferPtr = IntPtr.Zero;

        bufferDescData.Name = strAlloc.Acquire(bufferDesc.Name);
        Interop.render_device_create_buffer(Handle,
            new IntPtr(&bufferDescData),
            IntPtr.Zero,
            new IntPtr(&bufferPtr));

        return DiligentObjectsFactory.CreateBuffer(bufferPtr);
    }

    public IBuffer CreateBuffer(BufferDesc bufferDesc, BufferData initialData)
    {
        using var strAlloc = new StringAllocator();

        var bufferDescData = BufferDesc.GetInternalStruct(bufferDesc);
        var bufferDataStruct = BufferData.GetInternalStruct(new BufferData());
        var bufferPtr = IntPtr.Zero;

        bufferDescData.Name = strAlloc.Acquire(bufferDesc.Name);
        Interop.render_device_create_buffer(
            Handle,
            new IntPtr(&bufferDescData),
            new IntPtr(&bufferDataStruct),
            new IntPtr(&bufferPtr));

        return DiligentObjectsFactory.CreateBuffer(bufferPtr);
    }

    public IBuffer CreateBuffer<T>(BufferDesc bufferDesc, T initialData) where T : unmanaged
    {
        return CreateBuffer(bufferDesc, new BufferData()
        {
            Data = new IntPtr(&initialData)
        });
    }

    public IBuffer CreateBuffer<T>(BufferDesc bufferDesc, ReadOnlySpan<T> initialData) where T : unmanaged
    {
        fixed (void* initialDataPtr = initialData)
            return CreateBuffer(bufferDesc, new BufferData()
            {
                Data = new IntPtr(&initialDataPtr)
            });
    }

    public IShader CreateShader(ShaderCreateInfo createInfo, out IDataBlob compilerOutput)
    {
        using var strAlloc = new StringAllocator();
        var createInfoData = ShaderCreateInfo.GetInternalStruct(createInfo);
        var shaderPtr = IntPtr.Zero;
        var compilerOutputPtr = IntPtr.Zero;

        createInfoData.Desc.Name = strAlloc.Acquire(createInfo.Desc.Name);
        createInfoData.Source = strAlloc.Acquire(createInfo.Source);
        createInfoData.EntryPoint = strAlloc.Acquire(createInfo.EntryPoint);

        var byteCode = createInfo.ByteCodeData.AsSpan();
        fixed (void* byteCodePtr = byteCode)
        {
            if (createInfo.ByteCode == IntPtr.Zero)
            {
                createInfoData.ByteCode = new IntPtr(byteCodePtr);
                createInfoData.ByteCodeSize = (uint)createInfo.ByteCodeData.Length;
            }
            else
            {
                createInfoData.ByteCode = createInfo.ByteCode;
                createInfoData.ByteCodeSize = createInfo.ByteCodeSize;
            }

            Interop.render_device_create_shader(Handle,
                new IntPtr(&createInfoData),
                new IntPtr(&shaderPtr),
                new IntPtr(&compilerOutputPtr));
        }

        compilerOutput = DiligentObjectsFactory.CreateDataBlob(compilerOutputPtr);
        return DiligentObjectsFactory.CreateShader(shaderPtr);
    }

    public ITexture CreateTexture(TextureDesc textureDesc)
    {
        using var strAlloc = new StringAllocator();
        var textureDescData = TextureDesc.GetInternalStruct(textureDesc);
        var texturePtr = IntPtr.Zero;

        textureDescData.Name = strAlloc.Acquire(textureDesc.Name);
        Interop.render_device_create_texture(Handle,
            new IntPtr(&textureDescData),
            IntPtr.Zero,
            new IntPtr(&texturePtr));
        return DiligentObjectsFactory.CreateTexture(texturePtr);
    }

    public ITexture CreateTexture(TextureDesc textureDesc, TextureData textureData)
    {
        using var strAlloc = new StringAllocator();
        var textureDescData = TextureDesc.GetInternalStruct(textureDesc);
        var textureDataStruct = TextureData.GetInternalStruct(textureData);
        var texturePtr = IntPtr.Zero;

        var subResourcesData = textureData.SubResources.Select(x => TextureSubResData.GetInternalStruct(x)).ToArray();
        textureDescData.Name = strAlloc.Acquire(textureDesc.Name);

        fixed (void* subResourcesPtr = subResourcesData.AsSpan())
        {
            textureDataStruct.NumSubresources = (uint)subResourcesData.Length;
            textureDataStruct.pSubResources = new IntPtr(subResourcesPtr);
            Interop.render_device_create_texture(Handle,
                new IntPtr(&textureDescData),
                new IntPtr(&textureDataStruct),
                new IntPtr(&texturePtr));
        }

        return DiligentObjectsFactory.CreateTexture(texturePtr);
    }

    public ISampler CreateSampler(SamplerDesc samplerDesc)
    {
        var samplerDescData = SamplerDesc.GetInternalStruct(samplerDesc);
        var samplerPtr = IntPtr.Zero;

        Interop.render_device_create_sampler(Handle, new IntPtr(&samplerDescData), new IntPtr(&samplerPtr));
        return DiligentObjectsFactory.CreateSampler(samplerPtr);
    }

    public IResourceMapping CreateResourceMapping(ResourceMappingCreateInfo createInfo)
    {
        using var strAlloc = new StringAllocator();
        var createInfoData = ResourceMappingCreateInfo.GetInternalStruct(createInfo);
        var entriesData = createInfo.Entries.Select(x =>
        {
            var entryData = ResourceMappingEntry.GetInternalStruct(x);
            entryData.Name = strAlloc.Acquire(x.Name);
            return entryData;
        }).ToArray().AsSpan();
        var resourceMappingPtr = IntPtr.Zero;

        fixed (void* entriesPtr = entriesData)
        {
            createInfoData.pEntries = new IntPtr(entriesPtr);
            Interop.render_device_create_resource_mapping(Handle,
                new IntPtr(&createInfoData),
                new IntPtr(&resourceMappingPtr));
        }

        return DiligentObjectsFactory.CreateResourceMapping(resourceMappingPtr);
    }

    public IPipelineState CreateGraphicsPipelineState(GraphicsPipelineStateCreateInfo createInfo)
    {
        throw new NotImplementedException();
    }

    public IPipelineState CreateComputePipelineState(ComputePipelineStateCreateInfo createInfo)
    {
        throw new NotImplementedException();
    }

    public IPipelineState CreateRayTracingPipelineState(RayTracingPipelineStateCreateInfo createInfo)
    {
        throw new NotImplementedException();
    }

    public IPipelineState CreateTilePipelineState(TilePipelineStateCreateInfo createInfo)
    {
        throw new NotImplementedException();
    }

    public IFence CreateFence(FenceDesc desc)
    {
        throw new NotImplementedException();
    }

    public IQuery CreateQuery(QueryDesc queryDesc)
    {
        throw new NotImplementedException();
    }

    public IRenderPass CreateRenderPass(RenderPassDesc desc)
    {
        throw new NotImplementedException();
    }

    public IFramebuffer CreateFramebuffer(FramebufferDesc desc)
    {
        throw new NotImplementedException();
    }

    public IBottomLevelAS CreateBLAS(BottomLevelASDesc desc)
    {
        throw new NotImplementedException();
    }

    public ITopLevelAS CreateTLAS(TopLevelASDesc desc)
    {
        throw new NotImplementedException();
    }

    public IShaderBindingTable CreateSBT(ShaderBindingTableDesc desc)
    {
        throw new NotImplementedException();
    }

    public IPipelineResourceSignature CreatePipelineResourceSignature(PipelineResourceSignatureDesc desc)
    {
        throw new NotImplementedException();
    }

    public IDeviceMemory CreateDeviceMemory(DeviceMemoryDesc createInfo)
    {
        throw new NotImplementedException();
    }

    public IPipelineStateCache CreatePipelineStateCache(PipelineStateCacheCreateInfo createInfo)
    {
        throw new NotImplementedException();
    }

    public TextureFormatInfo GetTextureFormatInfo(TextureFormat format)
    {
        throw new NotImplementedException();
    }

    public TextureFormatInfoExt GetTextureFormatInfoExt(TextureFormat format)
    {
        throw new NotImplementedException();
    }

    public SparseTextureFormatInfo GetSparseTextureFormatInfo(TextureFormat format, ResourceDimension dimension,
        uint sampleCount)
    {
        throw new NotImplementedException();
    }

    public void ReleaseStaleResources(bool forceRelease = false)
    {
        throw new NotImplementedException();
    }

    public void IdleGPU()
    {
        throw new NotImplementedException();
    }
}
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
        var bufferDataStruct = BufferData.GetInternalStruct(initialData);
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
            Data = new IntPtr(&initialData),
            DataSize = (ulong)Unsafe.SizeOf<T>()
        });
    }

    public IBuffer CreateBuffer<T>(BufferDesc bufferDesc, ReadOnlySpan<T> initialData) where T : unmanaged
    {
        fixed (void* initialDataPtr = initialData)
            return CreateBuffer(bufferDesc,
                new BufferData()
                {
                    Data = new IntPtr(&initialDataPtr), 
                    DataSize = (ulong)(initialData.Length * Unsafe.SizeOf<T>())
                });
    }

    public IBuffer CreateBuffer<T>(BufferDesc bufferDesc, Span<T> initialData) where T : unmanaged
    {
        fixed (void* initialDataPtr = initialData)
            return CreateBuffer(bufferDesc,
                new BufferData()
                {
                    Data = new IntPtr(initialDataPtr), 
                    DataSize = (ulong)(initialData.Length * Unsafe.SizeOf<T>())
                });
    }

    public IShader CreateShader(ShaderCreateInfo createInfo, out IDataBlob? compilerOutput)
    {
        using var strAlloc = new StringAllocator();
        var createInfoData = ShaderCreateInfo.GetInternalStruct(createInfo);
        var shaderPtr = IntPtr.Zero;
        var compilerOutputPtr = IntPtr.Zero;
        var shaderMacros = createInfo.Macros.Select(x =>
        {
            var data = ShaderMacro.GetInternalStruct(x);
            data.Name = strAlloc.Acquire(x.Name);
            data.Definition = strAlloc.Acquire(x.Definition);
            return data;
        }).ToArray();

        createInfoData.Desc.Name = strAlloc.Acquire(createInfo.Desc.Name);
        createInfoData.Source = strAlloc.Acquire(createInfo.Source);
        createInfoData.EntryPoint = strAlloc.Acquire(createInfo.EntryPoint);

        var byteCode = createInfo.ByteCodeData.AsSpan();
        fixed(void* shaderMacroPtr = shaderMacros.AsSpan())
        fixed (void* byteCodePtr = byteCode)
        {
            createInfoData.Macros = new ShaderMacroArray.__Internal()
            {
                Count = (uint)createInfo.Macros.Length,
                Elements = new IntPtr(shaderMacroPtr)
            };
            
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

        compilerOutput = compilerOutputPtr != IntPtr.Zero 
            ? DiligentObjectsFactory.CreateDataBlob(compilerOutputPtr)
            : null;
        return DiligentObjectsFactory.CreateShader(shaderPtr);
    }

    public IShader CreateShader(ShaderCreateInfo createInfo)
    {
        var result = CreateShader(createInfo, out var compilerOutput);
        compilerOutput?.Dispose();
        return result;
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
        using var strAlloc = new StringAllocator();
        var samplerDescData = SamplerDesc.GetInternalStruct(samplerDesc);
        samplerDescData.Name = strAlloc.Acquire(samplerDesc.Name);

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
        using var strAlloc = new StringAllocator();
        var createInfoData = GraphicsPipelineStateCreateInfo.GetInternalStruct(createInfo);
        var resourceSignatures = createInfo.ResourceSignatures
            .Select(x => x.Handle)
            .ToArray()
            .AsSpan();
        var variables = createInfo.PSODesc.ResourceLayout.Variables
            .Select(x =>
            {
                var result = ShaderResourceVariableDesc.GetInternalStruct(x);
                result.Name = strAlloc.Acquire(x.Name);
                return result;
            })
            .ToArray()
            .AsSpan();
        var immutableSamplers = createInfo.PSODesc.ResourceLayout.ImmutableSamplers
            .Select(x =>
            {
                var result = ImmutableSamplerDesc.GetInternalStruct(x);
                result.SamplerOrTextureName = strAlloc.Acquire(x.SamplerOrTextureName);
                return result;
            })
            .ToArray()
            .AsSpan();
        var inputLayout = createInfo.GraphicsPipeline.InputLayout.LayoutElements
            .Select(x =>
            {
                var result = LayoutElement.GetInternalStruct(x);
                result.HLSLSemantic = strAlloc.Acquire(x.HLSLSemantic);
                return result;
            })
            .ToArray()
            .AsSpan();

        var pipelineStatePtr = IntPtr.Zero;
        createInfoData.PSODesc.PipelineType = PipelineType.Graphics;
        createInfoData.PSODesc.Name = strAlloc.Acquire(createInfo.PSODesc.Name);

        fixed (void* resourceSignaturesPtr = resourceSignatures)
        fixed (void* variablesPtr = variables)
        fixed (void* immutableSamplersPtr = immutableSamplers)
        fixed (void* inputLayoutPtr = inputLayout)
        {
            createInfoData.PSODesc.ResourceLayout.Variables = new IntPtr(variablesPtr);
            createInfoData.PSODesc.ResourceLayout.ImmutableSamplers = new IntPtr(immutableSamplersPtr);

            createInfoData.ppResourceSignatures = new IntPtr(resourceSignaturesPtr);
            createInfoData.ResourceSignaturesCount = (uint)createInfo.ResourceSignatures.Length;

            createInfoData.GraphicsPipeline.InputLayout.LayoutElements = new IntPtr(inputLayoutPtr);
            createInfoData.GraphicsPipeline.InputLayout.NumElements =
                (uint)createInfo.GraphicsPipeline.InputLayout.LayoutElements.Length;

            Interop.render_device_create_graphics_pipeline_state(Handle,
                new IntPtr(&createInfoData),
                new IntPtr(&pipelineStatePtr));
        }

        return DiligentObjectsFactory.CreatePipelineState(pipelineStatePtr);
    }

    public IPipelineState CreateComputePipelineState(ComputePipelineStateCreateInfo createInfo)
    {
        using var strAlloc = new StringAllocator();
        var createInfoData = ComputePipelineStateCreateInfo.GetInternalStruct(createInfo);
        var resourceSignatures = createInfo.ResourceSignatures
            .Select(x => x.Handle)
            .ToArray()
            .AsSpan();
        var variables = createInfo.PSODesc.ResourceLayout.Variables
            .Select(x =>
            {
                var result = ShaderResourceVariableDesc.GetInternalStruct(x);
                result.Name = strAlloc.Acquire(x.Name);
                return result;
            })
            .ToArray()
            .AsSpan();
        var immutableSamplers = createInfo.PSODesc.ResourceLayout.ImmutableSamplers
            .Select(x =>
            {
                var result = ImmutableSamplerDesc.GetInternalStruct(x);
                result.SamplerOrTextureName = strAlloc.Acquire(x.SamplerOrTextureName);
                return result;
            })
            .ToArray()
            .AsSpan();

        var pipelineStatePtr = IntPtr.Zero;
        createInfoData.PSODesc.PipelineType = PipelineType.Compute;
        createInfoData.PSODesc.Name = strAlloc.Acquire(createInfo.PSODesc.Name);

        fixed (void* resourceSignaturesPtr = resourceSignatures)
        fixed (void* variablesPtr = variables)
        fixed (void* immutableSamplersPtr = immutableSamplers)
        {
            createInfoData.ppResourceSignatures = new IntPtr(resourceSignaturesPtr);
            createInfoData.ResourceSignaturesCount = (uint)createInfo.ResourceSignatures.Length;

            createInfoData.PSODesc.ResourceLayout.Variables = new IntPtr(variablesPtr);
            createInfoData.PSODesc.ResourceLayout.NumVariables =
                (uint)createInfo.PSODesc.ResourceLayout.Variables.Length;
            createInfoData.PSODesc.ResourceLayout.ImmutableSamplers = new IntPtr(immutableSamplersPtr);
            createInfoData.PSODesc.ResourceLayout.NumImmutableSamplers =
                (uint)createInfo.PSODesc.ResourceLayout.ImmutableSamplers.Length;

            Interop.render_device_create_graphics_pipeline_state(Handle,
                new IntPtr(&createInfoData),
                new IntPtr(&pipelineStatePtr));
        }

        return DiligentObjectsFactory.CreatePipelineState(pipelineStatePtr);
    }

    public IPipelineState CreateRayTracingPipelineState(RayTracingPipelineStateCreateInfo createInfo)
    {
        using var strAlloc = new StringAllocator();
        var createInfoData = RayTracingPipelineStateCreateInfo.GetInternalStruct(createInfo);
        var resourceSignatures = createInfo.ResourceSignatures
            .Select(x => x.Handle)
            .ToArray()
            .AsSpan();
        var variables = createInfo.PSODesc.ResourceLayout.Variables
            .Select(x =>
            {
                var result = ShaderResourceVariableDesc.GetInternalStruct(x);
                result.Name = strAlloc.Acquire(x.Name);
                return result;
            })
            .ToArray()
            .AsSpan();
        var immutableSamplers = createInfo.PSODesc.ResourceLayout.ImmutableSamplers
            .Select(x =>
            {
                var result = ImmutableSamplerDesc.GetInternalStruct(x);
                result.SamplerOrTextureName = strAlloc.Acquire(x.SamplerOrTextureName);
                return result;
            })
            .ToArray()
            .AsSpan();
        var generalShaders = createInfo.GeneralShaders
            .Select(x =>
            {
                var result = RayTracingGeneralShaderGroup.GetInternalStruct(x);
                result.Name = strAlloc.Acquire(x.Name);
                return result;
            })
            .ToArray()
            .AsSpan();
        var triangleHitShaders = createInfo.TriangleHitShaders
            .Select(x =>
            {
                var result = RayTracingTriangleHitShaderGroup.GetInternalStruct(x);
                result.Name = strAlloc.Acquire(x.Name);
                return result;
            })
            .ToArray()
            .AsSpan();
        var proceduralHitShaders = createInfo.ProceduralHitShaders
            .Select(x =>
            {
                var result = RayTracingProceduralHitShaderGroup.GetInternalStruct(x);
                result.Name = strAlloc.Acquire(x.Name);
                return result;
            })
            .ToArray()
            .AsSpan();

        var pipelineStatePtr = IntPtr.Zero;
        createInfoData.PSODesc.PipelineType = PipelineType.RayTracing;
        createInfoData.PSODesc.Name = strAlloc.Acquire(createInfo.PSODesc.Name);
        createInfoData.pShaderRecordName = strAlloc.Acquire(createInfo.pShaderRecordName);

        fixed (void* resourceSignaturesPtr = resourceSignatures)
        fixed (void* variablesPtr = variables)
        fixed (void* immutableSamplersPtr = immutableSamplers)
        fixed (void* generalShadersPtr = generalShaders)
        fixed (void* triangleHitShadersPtr = triangleHitShaders)
        fixed (void* proceduralHitShadersPtr = proceduralHitShaders)
        {
            createInfoData.ppResourceSignatures = new IntPtr(resourceSignaturesPtr);
            createInfoData.ResourceSignaturesCount = (uint)createInfo.ResourceSignatures.Length;

            createInfoData.PSODesc.ResourceLayout.Variables = new IntPtr(variablesPtr);
            createInfoData.PSODesc.ResourceLayout.NumVariables =
                (uint)createInfo.PSODesc.ResourceLayout.Variables.Length;

            createInfoData.PSODesc.ResourceLayout.ImmutableSamplers = new IntPtr(immutableSamplersPtr);
            createInfoData.PSODesc.ResourceLayout.NumImmutableSamplers =
                (uint)createInfo.PSODesc.ResourceLayout.ImmutableSamplers.Length;

            createInfoData.pGeneralShaders = new IntPtr(generalShadersPtr);
            createInfoData.GeneralShaderCount = (uint)createInfo.GeneralShaders.Length;

            createInfoData.pTriangleHitShaders = new IntPtr(triangleHitShadersPtr);
            createInfoData.TriangleHitShaderCount = (uint)createInfo.TriangleHitShaders.Length;

            createInfoData.pProceduralHitShaders = new IntPtr(proceduralHitShadersPtr);
            createInfoData.ProceduralHitShaderCount = (uint)createInfo.ProceduralHitShaders.Length;

            Interop.render_device_create_ray_tracing_pipeline_state(Handle,
                new IntPtr(&createInfoData),
                new IntPtr(&pipelineStatePtr));
        }

        return DiligentObjectsFactory.CreatePipelineState(pipelineStatePtr);
    }

    public IPipelineState CreateTilePipelineState(TilePipelineStateCreateInfo createInfo)
    {
        using var strAlloc = new StringAllocator();
        var createInfoData = TilePipelineStateCreateInfo.GetInternalStruct(createInfo);
        var resourceSignatures = createInfo.ResourceSignatures
            .Select(x => x.Handle)
            .ToArray()
            .AsSpan();
        var variables = createInfo.PSODesc.ResourceLayout.Variables
            .Select(x =>
            {
                var result = ShaderResourceVariableDesc.GetInternalStruct(x);
                result.Name = strAlloc.Acquire(x.Name);
                return result;
            })
            .ToArray()
            .AsSpan();
        var immutableSamplers = createInfo.PSODesc.ResourceLayout.ImmutableSamplers
            .Select(x =>
            {
                var result = ImmutableSamplerDesc.GetInternalStruct(x);
                result.SamplerOrTextureName = strAlloc.Acquire(x.SamplerOrTextureName);
                return result;
            })
            .ToArray()
            .AsSpan();

        var pipelineStatePtr = IntPtr.Zero;
        createInfoData.PSODesc.PipelineType = PipelineType.Tile;
        createInfoData.PSODesc.Name = strAlloc.Acquire(createInfo.PSODesc.Name);

        fixed (void* resourceSignaturesPtr = resourceSignatures)
        fixed (void* variablesPtr = variables)
        fixed (void* immutableSamplersPtr = immutableSamplers)
        {
            createInfoData.ppResourceSignatures = new IntPtr(resourceSignaturesPtr);
            createInfoData.ResourceSignaturesCount = (uint)createInfo.ResourceSignatures.Length;

            createInfoData.PSODesc.ResourceLayout.Variables = new IntPtr(variablesPtr);
            createInfoData.PSODesc.ResourceLayout.NumVariables =
                (uint)createInfo.PSODesc.ResourceLayout.Variables.Length;

            createInfoData.PSODesc.ResourceLayout.ImmutableSamplers = new IntPtr(immutableSamplersPtr);
            createInfoData.PSODesc.ResourceLayout.NumImmutableSamplers =
                (uint)createInfo.PSODesc.ResourceLayout.ImmutableSamplers.Length;

            Interop.render_device_create_tile_pipeline_state(Handle,
                new IntPtr(&createInfoData),
                new IntPtr(&pipelineStatePtr));
        }

        return DiligentObjectsFactory.CreatePipelineState(pipelineStatePtr);
    }

    public IFence CreateFence(FenceDesc desc)
    {
        using var strAlloc = new StringAllocator();
        var descData = FenceDesc.GetInternalStruct(desc);

        var fencePtr = IntPtr.Zero;
        descData.Name = strAlloc.Acquire(desc.Name);

        Interop.render_device_create_fence(Handle, new IntPtr(&descData), new IntPtr(&fencePtr));
        return DiligentObjectsFactory.CreateFence(fencePtr);
    }

    public IQuery CreateQuery(QueryDesc queryDesc)
    {
        using var strAlloc = new StringAllocator();
        var queryDescData = QueryDesc.GetInternalStruct(queryDesc);
        var queryPtr = IntPtr.Zero;
        queryDescData.Name = strAlloc.Acquire(queryDesc.Name);

        Interop.render_device_create_query(Handle, new IntPtr(&queryDescData), new IntPtr(&queryPtr));
        return DiligentObjectsFactory.CreateQuery(Handle);
    }

    public IRenderPass CreateRenderPass(RenderPassDesc desc)
    {
        var descData = DiligentDescFactory.GetRenderPassDescBytes(desc);
        var renderPassPtr = IntPtr.Zero;

        fixed (void* descPtr = descData)
            Interop.render_device_create_render_pass(Handle,
                new IntPtr(descPtr),
                new IntPtr(&renderPassPtr));

        return DiligentObjectsFactory.CreateRenderPass(renderPassPtr);
    }

    public IFramebuffer CreateFramebuffer(FramebufferDesc desc)
    {
        using var strAlloc = new StringAllocator();
        var descData = FramebufferDesc.GetInternalStruct(desc);
        var attachments = desc.Attachments
            .Select(x => x.Handle)
            .ToArray()
            .AsSpan();

        var frameBufferPtr = IntPtr.Zero;
        descData.Name = strAlloc.Acquire(desc.Name);

        fixed (void* attachmentsPtr = attachments)
        {
            descData.ppAttachments = new IntPtr(attachmentsPtr);
            descData.AttachmentCount = (uint)desc.Attachments.Length;

            Interop.render_device_create_framebuffer(Handle,
                new IntPtr(&descData),
                new IntPtr(&frameBufferPtr));
        }

        return DiligentObjectsFactory.CreateFramebuffer(frameBufferPtr);
    }

    public IBottomLevelAS CreateBLAS(BottomLevelASDesc desc)
    {
        using var strAlloc = new StringAllocator();
        var descData = BottomLevelASDesc.GetInternalStruct(desc);
        var triangles = desc.Triangles
            .Select(x =>
            {
                var result = BLASTriangleDesc.GetInternalStruct(x);
                result.GeometryName = strAlloc.Acquire(x.GeometryName);
                return result;
            })
            .ToArray()
            .AsSpan();
        var boxes = desc.Boxes
            .Select(x =>
            {
                var result = BLASBoundingBoxDesc.GetInternalStruct(x);
                result.GeometryName = strAlloc.Acquire(x.GeometryName);
                return result;
            })
            .ToArray()
            .AsSpan();

        var bottomLevelAsPtr = IntPtr.Zero;
        descData.Name = strAlloc.Acquire(desc.Name);

        fixed (void* trianglesPtr = triangles)
        fixed (void* boxesPtr = boxes)
        {
            descData.pTriangles = new IntPtr(trianglesPtr);
            descData.TriangleCount = (uint)desc.Triangles.Length;

            descData.pBoxes = new IntPtr(boxesPtr);
            descData.BoxCount = (uint)desc.Boxes.Length;

            Interop.render_device_create_blas(Handle,
                new IntPtr(&descData),
                new IntPtr(&bottomLevelAsPtr));
        }

        return DiligentObjectsFactory.CreateBLAS(bottomLevelAsPtr);
    }

    public ITopLevelAS CreateTLAS(TopLevelASDesc desc)
    {
        using var strAlloc = new StringAllocator();
        var descData = TopLevelASDesc.GetInternalStruct(desc);
        descData.Name = strAlloc.Acquire(desc.Name);

        var topLevelASPtr = IntPtr.Zero;
        Interop.render_device_create_tlas(Handle,
            new IntPtr(&descData),
            new IntPtr(&topLevelASPtr));
        return DiligentObjectsFactory.CreateTLAS(topLevelASPtr);
    }

    public IShaderBindingTable CreateSBT(ShaderBindingTableDesc desc)
    {
        using var strAlloc = new StringAllocator();
        var descData = ShaderBindingTableDesc.GetInternalStruct(desc);
        descData.Name = strAlloc.Acquire(desc.Name);

        var shaderBindingTblPtr = IntPtr.Zero;
        Interop.render_device_create_sbt(Handle,
            new IntPtr(&descData),
            new IntPtr(&shaderBindingTblPtr));
        return DiligentObjectsFactory.CreateSBT(shaderBindingTblPtr);
    }

    public IPipelineResourceSignature CreatePipelineResourceSignature(PipelineResourceSignatureDesc desc)
    {
        using var strAlloc = new StringAllocator();
        var descData = PipelineResourceSignatureDesc.GetInternalStruct(desc);
        var resources = desc.Resources
            .Select(x =>
            {
                var result = PipelineResourceDesc.GetInternalStruct(x);
                result.Name = strAlloc.Acquire(x.Name);
                return result;
            })
            .ToArray()
            .AsSpan();
        var immutableSamplers = desc.ImmutableSamplers
            .Select(x =>
            {
                var result = ImmutableSamplerDesc.GetInternalStruct(x);
                result.SamplerOrTextureName = strAlloc.Acquire(x.SamplerOrTextureName);
                result.Desc.Name = strAlloc.Acquire(x.Desc.Name);
                return result;
            })
            .ToArray()
            .AsSpan();

        var resourceSignaturePtr = IntPtr.Zero;

        descData.Name = strAlloc.Acquire(desc.Name);
        descData.CombinedSamplerSuffix = strAlloc.Acquire(desc.CombinedSamplerSuffix);

        fixed (void* resourcesPtr = resources)
        fixed (void* immutableSamplersPtr = immutableSamplers)
        {
            descData.Resources = new IntPtr(resourcesPtr);
            descData.NumResources = (uint)desc.Resources.Length;

            descData.ImmutableSamplers = new IntPtr(immutableSamplersPtr);
            descData.NumImmutableSamplers = (uint)desc.ImmutableSamplers.Length;

            Interop.render_device_create_pipeline_resource_signature(Handle,
                new IntPtr(&descData),
                new IntPtr(&resourceSignaturePtr));
        }

        return DiligentObjectsFactory.CreatePipelineResourceSignature(resourceSignaturePtr);
    }

    public IDeviceMemory CreateDeviceMemory(DeviceMemoryCreateInfo createInfo)
    {
        using var strAlloc = new StringAllocator();
        var createInfoData = DeviceMemoryCreateInfo.GetInternalStruct(createInfo);
        var compatibleResources = createInfo.CompatibleResources
            .Select(x => x.Handle)
            .ToArray()
            .AsSpan();

        var deviceMemoryPtr = IntPtr.Zero;
        createInfoData.Desc.Name = strAlloc.Acquire(createInfo.Desc.Name);

        fixed (void* compatibleResourcesPtr = compatibleResources)
        {
            createInfoData.ppCompatibleResources = new IntPtr(compatibleResourcesPtr);
            createInfoData.NumResources = (uint)createInfo.CompatibleResources.Length;

            Interop.render_device_create_device_memory(Handle,
                new IntPtr(&createInfoData),
                new IntPtr(&deviceMemoryPtr));
        }

        return DiligentObjectsFactory.CreateDeviceMemory(deviceMemoryPtr);
    }

    public IPipelineStateCache CreatePipelineStateCache(PipelineStateCacheCreateInfo createInfo)
    {
        using var strAlloc = new StringAllocator();
        var createInfoData = PipelineStateCacheCreateInfo.GetInternalStruct(createInfo);
        var cacheDataBytes = createInfo.CacheDataBytes.AsSpan();

        var pipelineStateCachePtr = IntPtr.Zero;

        createInfoData.Desc.Name = strAlloc.Acquire(createInfo.Desc.Name);
        fixed (void* cacheDataPtr = cacheDataBytes)
        {
            if (createInfo.CacheDataBytes.Any())
            {
                createInfoData.pCacheData = new IntPtr(cacheDataPtr);
                createInfoData.CacheDataSize = (uint)createInfo.CacheDataBytes.Length;
            }

            Interop.render_device_create_pipeline_state_cache(Handle,
                new IntPtr(&createInfoData),
                new IntPtr(&pipelineStateCachePtr));
        }

        return DiligentObjectsFactory.CreatePipelineStateCache(pipelineStateCachePtr);
    }

    public TextureFormatInfo GetTextureFormatInfo(TextureFormat format)
    {
        return DiligentDescFactory.GetTextureFormatInfo(
            Interop.render_device_get_texture_format_info(Handle, format)
        );
    }

    public TextureFormatInfoExt GetTextureFormatInfoExt(TextureFormat format)
    {
        return DiligentDescFactory.GetTextureFormatInfoExt(
            Interop.render_device_get_texture_format_info_ext(Handle, format)
        );
    }

    public SparseTextureFormatInfo GetSparseTextureFormatInfo(TextureFormat format, ResourceDimension dimension,
        uint sampleCount)
    {
        return DiligentDescFactory.GetSparseTextureFormatInfo(
            Interop.render_device_get_sparse_texture_format_info(Handle,
                format,
                dimension,
                sampleCount)
        );
    }

    public void ReleaseStaleResources(bool forceRelease = false)
    {
        Interop.render_device_release_stale_resources(Handle, forceRelease);
    }

    public void IdleGPU()
    {
        Interop.render_device_idle_gpu(Handle);
    }
}
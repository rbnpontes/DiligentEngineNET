namespace Diligent;

public interface IRenderDevice : IDiligentObject
{
     IEngineFactory EngineFactory { get; }
     RenderDeviceInfo DeviceInfo { get; }
     GraphicsAdapterInfo AdapterInfo { get; }
     IThreadPool ShaderCompilationThreadPool { get; }
     IBuffer CreateBuffer(BufferDesc bufferDesc);
     IBuffer CreateBuffer(BufferDesc bufferDesc, BufferData initialData);
     IBuffer CreateBuffer<T>(BufferDesc bufferDesc, T initialData) where T : unmanaged;
     IBuffer CreateBuffer<T>(BufferDesc bufferDesc, ReadOnlySpan<T> initialData) where T : unmanaged;
     IShader CreateShader(ShaderCreateInfo createInfo, out IDataBlob compilerOutput);
     ITexture CreateTexture(TextureDesc textureDesc);
     ITexture CreateTexture(TextureDesc textureDesc, TextureData textureData);
     ISampler CreateSampler(SamplerDesc samplerDesc);
     IResourceMapping CreateResourceMapping(ResourceMappingCreateInfo createInfo);
     IPipelineState CreateGraphicsPipelineState(GraphicsPipelineStateCreateInfo createInfo);
     IPipelineState CreateComputePipelineState(ComputePipelineStateCreateInfo createInfo);
     IPipelineState CreateRayTracingPipelineState(RayTracingPipelineStateCreateInfo createInfo);
     IPipelineState CreateTilePipelineState(TilePipelineStateCreateInfo createInfo);
     IFence CreateFence(FenceDesc desc);
     IQuery CreateQuery(QueryDesc queryDesc);
     IRenderPass CreateRenderPass(RenderPassDesc desc);
     IFramebuffer CreateFramebuffer(FramebufferDesc desc);
     IBottomLevelAS CreateBLAS(BottomLevelASDesc desc);
     ITopLevelAS CreateTLAS(TopLevelASDesc desc);
     IShaderBindingTable CreateSBT(ShaderBindingTableDesc desc);
     IPipelineResourceSignature CreatePipelineResourceSignature(PipelineResourceSignatureDesc desc);
     IDeviceMemory CreateDeviceMemory(DeviceMemoryDesc createInfo);
     IPipelineStateCache CreatePipelineStateCache(PipelineStateCacheCreateInfo createInfo);
     TextureFormatInfo GetTextureFormatInfo(TextureFormat format);
     TextureFormatInfoExt GetTextureFormatInfoExt(TextureFormat format);
     SparseTextureFormatInfo GetSparseTextureFormatInfo(TextureFormat format, ResourceDimension dimension,
          uint sampleCount);
     void IdleGPU();
}
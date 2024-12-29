namespace Diligent;

public unsafe interface IDeviceContext : IDiligentObject
{
    DeviceContextDesc Desc { get; }
    ulong FrameNumber { get; }
    IDiligentObject? UserData { get; set; }
    DeviceContextStats Stats { get; }

    void Begin(uint immediateContextId);
    void SetPipelineState(IPipelineState pipelineState);
    void TransitionShaderResources(IShaderResourceBinding shaderResourceBinding);

    void CommitShaderResources(IShaderResourceBinding shaderResourceBinding,
        ResourceStateTransitionMode stateTransitionMode);

    void SetStencilRef(uint stencilRef);
    void SetBlendFactors(float* blendFactors);
    void SetBlendFactors(float[] blendFactors);

    void SetVertexBuffers(uint startSlot, IBuffer[] buffers, ulong* offsets,
        ResourceStateTransitionMode stateTransitionMode, SetVertexBuffersFlags flags = SetVertexBuffersFlags.None);

    void SetVertexBuffers(uint startSlot, IBuffer[] buffers, ulong[] offsets,
        ResourceStateTransitionMode stateTransitionMode, SetVertexBuffersFlags flags = SetVertexBuffersFlags.None);

    void InvalidateState();

    void SetIndexBuffer(IBuffer indexBuffer, ulong byteOffset, ResourceStateTransitionMode stateTransitionMode);

    void SetViewports(Viewport[] viewports, uint rtWidth, uint rtHeight);

    void SetScissorRects(Rect[] rects, uint rtWidth, uint rtHeight);

    void SetRenderTargets(ITextureView[] renderTargets, ITextureView? depthStencil,
        ResourceStateTransitionMode stateTransitionMode);

    void SetRenderTargets(SetRenderTargetsAttribs attribs);
    void BeginRenderPass(BeginRenderPassAttribs attribs);
    void NextSubpass();
    void EndRenderPass();
    void Draw(DrawAttribs attribs);
    void DrawIndexed(DrawIndexedAttribs attribs);
    void DrawIndirect(DrawIndirectAttribs attribs);
    void DrawIndexedIndirect(DrawIndexedIndirectAttribs attribs);
    void DrawMesh(DrawMeshAttribs attribs);
    void DrawMeshIndirect(DrawMeshIndirectAttribs attribs);
    void MultiDraw(MultiDrawAttribs attribs);
    void MultiDrawIndexed(MultiDrawIndexedAttribs attribs);
    void DispatchCompute(DispatchComputeAttribs attribs);
    void DispatchComputeIndirect(DispatchComputeIndirectAttribs attribs);
    void DispatchTile(DispatchTileAttribs attribs);
    void GetTileSize(ref uint tileSizeX, ref uint tileSizeY);

    void ClearDepthStencil(ITextureView view, ClearDepthStencilFlags clearFlags, float depth, byte stencil,
        ResourceStateTransitionMode stateTransitionMode);

    void ClearRenderTarget(ITextureView view, float[] rgba, ResourceStateTransitionMode stateTransitionMode);
    void ClearRenderTarget(ITextureView view, uint[] rgba, ResourceStateTransitionMode stateTransitionMode);
    void ClearRenderTarget(ITextureView view, int[] rgba, ResourceStateTransitionMode stateTransitionMode);
    ICommandList FinishCommandList();
    void ExecuteCommandLists(ICommandList[] commandLists);
    void EnqueueSignal(IFence fence, ulong value);
    void DeviceWaitForFence(IFence fence, ulong value);
    void WaitForIdle();
    void BeginQuery(IQuery query);
    void EndQuery(IQuery query);
    void Flush();

    void UpdateBuffer(IBuffer buffer, ulong offset, ulong size, IntPtr data,
        ResourceStateTransitionMode stateTransitionMode);

    void UpdateBuffer<T>(IBuffer buffer, ulong offset, Span<T> data, ResourceStateTransitionMode stateTransitionMode)
        where T : unmanaged;

    void UpdateBuffer<T>(IBuffer buffer, ulong offset, ReadOnlySpan<T> data,
        ResourceStateTransitionMode stateTransitionMode) where T : unmanaged;

    void UpdateBuffer<T>(IBuffer buffer, ulong offset, T data, ResourceStateTransitionMode stateTransitionMode)
        where T : unmanaged;

    void CopyBuffer(IBuffer srcBuffer, ulong srcOffset, ResourceStateTransitionMode srcBufferTransitionMode,
        IBuffer dstBuffer, ulong dstOffset, ulong size, ResourceStateTransitionMode dstBufferTransitionMode);

    IntPtr MapBuffer(IBuffer buffer, MapType mapType, MapFlags mapFlags);
    void UnmapBuffer(IBuffer buffer, MapType mapType);

    void UpdateTexture(ITexture texture, uint mipLevel, uint slice, Box dstBox, TextureSubResData subResData,
        ResourceStateTransitionMode srcBufferTransitionMode, ResourceStateTransitionMode textureTransitionMode);

    void CopyTexture(CopyTextureAttribs copyAttribs);

    MappedTextureSubresource MapTextureSubresource(ITexture texture, uint mipLevel, uint arraySlice, MapType mapType,
        MapFlags mapFlags,
        Box? mapRegion);

    void UnmapTextureSubresource(ITexture texture, uint mipLevel, uint arraySlice);

    void GenerateMips(ITextureView textureView);
    void FinishFrame();
    void TransitionResourceStates(StateTransitionDesc[] resourceBarriers);

    void ResolveTextureSubresource(ITexture srcTexture, ITexture dstTexture,
        ResolveTextureSubresourceAttribs resolveAttribs);

    void BuildBLAS(BuildBLASAttribs attribs);
    void BuildTLAS(BuildTLASAttribs attribs);
    void CopyBLAS(CopyBLASAttribs attribs);
    void CopyTLAS(CopyTLASAttribs attribs);
    void WriteBLASCompactedSize(WriteBLASCompactedSizeAttribs attribs);
    void WriteTLASCompactedSize(WriteTLASCompactedSizeAttribs attribs);
    void TraceRays(TraceRaysAttribs attribs);
    void TraceRaysIndirect(TraceRaysIndirectAttribs attribs);
    void UpdateSBT(IShaderBindingTable sbt, UpdateIndirectRTBufferAttribs attribs);

    void BeginDebugGroup(string name);
    void BeginDebugGroup(string name, float* color);
    void BeginDebugGroup(string name, float[] color);
    void EndDebugGroup();
    void InsertDebugLabel(string label);
    void InsertDebugLabel(string label, float* color);
    void InsertDebugLabel(string label, float[] color);
    ICommandQueue LockCommandQueue();
    void UnlockCommandQueue();

    void SetShadingRate(ShadingRate baseRate, ShadingRateCombiner primitiveCombiner,
        ShadingRateCombiner textureCombiner);

    void BindSparseResourceMemory(BindSparseResourceMemoryAttribs attribs);
    void ClearStats();
}
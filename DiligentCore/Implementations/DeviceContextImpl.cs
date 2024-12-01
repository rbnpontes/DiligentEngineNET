using Diligent.Utils;

namespace Diligent;

internal unsafe partial class DeviceContext(IntPtr handle) : DiligentObject(handle), IDeviceContext
{
    /**
     * DevNotes: This class intentionally breaks some Clean Code principles
     * due to the specific demands of rendering performance.
     * Each jump, allocation, copy operation, and fixed instruction can significantly impact performance.
     * While such low-level optimizations are unnecessary in most cases,
     * every nanosecond counts in this context.
     */
    private IntPtr[] _tmpPointersBuffer = [];

    public DeviceContextDesc Desc => DiligentDescFactory.GetDeviceContextDesc(
        Interop.device_context_get_desc(Handle)
    );

    public ulong FrameNumber => Interop.device_context_get_frame_number(Handle);

    public IDiligentObject? UserData
    {
        get
        {
            var ptr = Interop.device_context_get_user_data(Handle);
            if (ptr == IntPtr.Zero)
                return null;
            NativeObjectRegistry.TryGetObject<IDiligentObject>(ptr, out var result);
            return result ?? new UnknownObject(ptr);
        }
        set => Interop.device_context_set_user_data(Handle, value?.Handle ?? IntPtr.Zero);
    }

    public DeviceContextStats Stats => DiligentDescFactory.GetDeviceContextStats(
        Interop.device_context_get_stats(Handle)
    );

    public void Begin(uint immediateContextId)
    {
        Interop.device_context_begin(Handle, immediateContextId);
    }

    public void SetPipelineState(IPipelineState pipelineState)
    {
        Interop.device_context_set_pipeline_state(Handle, pipelineState.Handle);
    }

    public void TransitionShaderResources(IShaderResourceBinding shaderResourceBinding)
    {
        Interop.device_context_transition_shader_resources(Handle, shaderResourceBinding.Handle);
    }

    public void CommitShaderResources(IShaderResourceBinding shaderResourceBinding,
        ResourceStateTransitionMode stateTransitionMode)
    {
        Interop.device_context_commit_shader_resources(Handle, shaderResourceBinding.Handle, stateTransitionMode);
    }

    public void SetStencilRef(uint stencilRef)
    {
        Interop.device_context_set_stencil_ref(Handle, stencilRef);
    }

    public unsafe void SetBlendFactors(float* blendFactors)
    {
        Interop.device_context_set_blend_factors(Handle, new IntPtr(blendFactors));
    }

    public void SetBlendFactors(float[] blendFactors)
    {
        fixed (float* blendFactorsPtr = blendFactors)
            SetBlendFactors(blendFactorsPtr);
    }

    public void SetVertexBuffers(uint startSlot, IBuffer[] buffers, ulong* offsets,
        ResourceStateTransitionMode stateTransitionMode, SetVertexBuffersFlags flags = SetVertexBuffersFlags.None)
    {
        if (buffers.Length > _tmpPointersBuffer.Length)
            _tmpPointersBuffer = new IntPtr[buffers.Length];

        fixed (nint* buffersPtr = _tmpPointersBuffer)
        {
            for (var i = 0; i < buffers.Length; ++i)
                buffersPtr[i] = buffers[i].Handle;
            Interop.device_context_set_vertex_buffers(Handle,
                startSlot,
                (uint)buffers.Length,
                new IntPtr(buffersPtr),
                new IntPtr(offsets),
                stateTransitionMode,
                flags);
        }
    }

    public void SetVertexBuffers(uint startSlot, IBuffer[] buffers, ulong[] offsets,
        ResourceStateTransitionMode stateTransitionMode, SetVertexBuffersFlags flags = SetVertexBuffersFlags.None)
    {
        fixed (ulong* offsetsPtr = offsets)
            SetVertexBuffers(startSlot, buffers, offsetsPtr, stateTransitionMode, flags);
    }

    public void InvalidateState()
    {
        Interop.device_context_invalidate_state(Handle);
    }

    public void SetIndexBuffer(IBuffer indexBuffer, ulong byteOffset, ResourceStateTransitionMode stateTransitionMode)
    {
        Interop.device_context_set_index_buffer(Handle, indexBuffer.Handle, byteOffset, stateTransitionMode);
    }

    private Viewport.__Internal[] _tmpViewports = [];

    public void SetViewports(Viewport[] viewports, uint rtWidth, uint rtHeight)
    {
        if (viewports.Length > _tmpViewports.Length)
            _tmpViewports = new Viewport.__Internal[viewports.Length];

        fixed (Viewport.__Internal* viewportsPtr = _tmpViewports)
        {
            for (var i = 0; i < viewports.Length; ++i)
                viewportsPtr[i] = Viewport.GetInternalStruct(viewports[i]);

            Interop.device_context_set_viewports(Handle,
                (uint)viewports.Length,
                new IntPtr(viewportsPtr),
                rtWidth,
                rtHeight);
        }
    }

    private Rect.__Internal[] _tmpRects = [];

    public void SetScissorRects(Rect[] rects, uint rtWidth, uint rtHeight)
    {
        if (rects.Length > _tmpRects.Length)
            _tmpRects = new Rect.__Internal[rects.Length];

        fixed (Rect.__Internal* rectsPtr = _tmpRects)
        {
            for (var i = 0; i < rects.Length; ++i)
                rectsPtr[i] = Rect.GetInternalStruct(rects[i]);
            Interop.device_context_set_scissor_rects(
                Handle,
                (uint)rects.Length,
                new IntPtr(rectsPtr),
                rtWidth,
                rtHeight);
        }
    }

    public void SetRenderTargets(ITextureView[] renderTargets, ITextureView? depthStencil,
        ResourceStateTransitionMode stateTransitionMode)
    {
        if (renderTargets.Length > _tmpPointersBuffer.Length)
            _tmpPointersBuffer = new IntPtr[renderTargets.Length];

        fixed (nint* renderTargetsPtr = _tmpPointersBuffer)
        {
            for (var i = 0; i < renderTargets.Length; ++i)
                renderTargetsPtr[i] = renderTargets[i].Handle;

            Interop.device_context_set_render_targets(Handle,
                (uint)renderTargets.Length,
                new IntPtr(renderTargetsPtr),
                depthStencil?.Handle ?? IntPtr.Zero,
                stateTransitionMode);
        }
    }

    public void SetRenderTargets(SetRenderTargetsAttribs attribs)
    {
        if (attribs.RenderTargets.Length > _tmpPointersBuffer.Length)
            _tmpPointersBuffer = new IntPtr[attribs.RenderTargets.Length];

        var data = SetRenderTargetsAttribs.GetInternalStruct(attribs);
        fixed (nint* renderTargetsPtr = _tmpPointersBuffer)
        {
            for (var i = 0; i < attribs.RenderTargets.Length; ++i)
                _tmpPointersBuffer[i] = attribs.RenderTargets[i].Handle;
            
            data.ppRenderTargets = new IntPtr(renderTargetsPtr);
            Interop.device_context_set_render_targets_ext(Handle, new IntPtr(&data));
        }
    }

    public void BeginRenderPass(BeginRenderPassAttribs attribs)
    {
        throw new NotImplementedException();
    }

    public void NextSubpass()
    {
        throw new NotImplementedException();
    }

    public void EndRenderPass()
    {
        throw new NotImplementedException();
    }

    public void Draw(DrawAttribs attribs)
    {
        throw new NotImplementedException();
    }

    public void DrawIndexed(DrawIndexedAttribs attribs)
    {
        throw new NotImplementedException();
    }

    public void DrawIndirect(DrawIndirectAttribs attribs)
    {
        throw new NotImplementedException();
    }

    public void DrawIndexedIndirect(DrawIndexedIndirectAttribs attribs)
    {
        throw new NotImplementedException();
    }

    public void DrawMesh(DrawMeshAttribs attribs)
    {
        throw new NotImplementedException();
    }

    public void DrawMeshIndirect(DrawMeshIndirectAttribs attribs)
    {
        throw new NotImplementedException();
    }

    public void MultiDrawIndexed(MultiDrawIndexedAttribs attribs)
    {
        throw new NotImplementedException();
    }

    public void DispatchCompute(DispatchComputeAttribs attribs)
    {
        throw new NotImplementedException();
    }

    public void DispatchComputeIndirect(DispatchComputeIndirectAttribs attribs)
    {
        throw new NotImplementedException();
    }

    public void DispatchTile(DispatchTileAttribs attribs)
    {
        throw new NotImplementedException();
    }

    public void GetTileSize(ref uint tileSizeX, ref uint tileSizeY)
    {
        throw new NotImplementedException();
    }

    public void ClearDepthStencil(ITextureView view, ClearDepthStencilFlags clearFlags, float depth, byte stencil,
        ResourceStateTransitionMode stateTransitionMode)
    {
        throw new NotImplementedException();
    }

    public void ClearRenderTarget(ITextureView view, float[] rgba, ResourceStateTransitionMode stateTransitionMode)
    {
        throw new NotImplementedException();
    }

    public void ClearRenderTarget(ITextureView view, uint[] rgba, ResourceStateTransitionMode stateTransitionMode)
    {
        throw new NotImplementedException();
    }

    public void ClearRenderTarget(ITextureView view, int[] rgba, ResourceStateTransitionMode stateTransitionMode)
    {
        throw new NotImplementedException();
    }

    public ICommandList FinishCommandList()
    {
        throw new NotImplementedException();
    }

    public void ExecuteCommandLists(ICommandList[] commandLists)
    {
        throw new NotImplementedException();
    }

    public void EnqueueSignal(IFence fence, ulong value)
    {
        throw new NotImplementedException();
    }

    public void DeviceWaitForFence(IFence fence, ulong value)
    {
        throw new NotImplementedException();
    }

    public void WaitForIdle()
    {
        throw new NotImplementedException();
    }

    public void BeginQuery(IQuery query)
    {
        throw new NotImplementedException();
    }

    public void EndQuery(IQuery query)
    {
        throw new NotImplementedException();
    }

    public void Flush()
    {
        throw new NotImplementedException();
    }

    public void CopyBuffer(IBuffer srcBuffer, ulong srcOffset, ResourceStateTransitionMode srcBufferTransitionMode,
        IBuffer dstBuffer, ulong dstOffset, ulong size, ResourceStateTransitionMode dstBufferTransitionMode)
    {
        throw new NotImplementedException();
    }

    public IntPtr MapBuffer(IBuffer buffer, MapType mapType, MapFlags mapFlags)
    {
        throw new NotImplementedException();
    }

    public void UnmapBuffer(IBuffer buffer, MapType mapType)
    {
        throw new NotImplementedException();
    }

    public void UpdateTexture(ITexture texture, uint mipLevel, uint slice, Box dstBox, TextureSubResData subResData,
        ResourceStateTransitionMode srcBufferTransitionMode, ResourceStateTransitionMode textureTransitionMode)
    {
        throw new NotImplementedException();
    }

    public void CopyTexture(CopyTextureAttribs copyAttribs)
    {
        throw new NotImplementedException();
    }

    public void MapTextureSubresource(ITexture texture, uint mipLevel, uint arraySlice, MapType mapType,
        MapFlags mapFlags,
        Box? mapRegion, MappedTextureSubresource mappedData)
    {
        throw new NotImplementedException();
    }

    public void GenerateMips(ITextureView textureView)
    {
        throw new NotImplementedException();
    }

    public void FinishFrame()
    {
        throw new NotImplementedException();
    }

    public void TransitionResourceStates(StateTransitionDesc[] resourceBarriers)
    {
        throw new NotImplementedException();
    }

    public void ResolveTextureSubresource(ITexture srcTexture, ITexture dstTexture,
        ResolveTextureSubresourceAttribs resolveAttribs)
    {
        throw new NotImplementedException();
    }

    public void BuildBLAS(BuildBLASAttribs attribs)
    {
        throw new NotImplementedException();
    }

    public void BuildTLAS(BuildTLASAttribs attribs)
    {
        throw new NotImplementedException();
    }

    public void CopyBLAS(CopyBLASAttribs attribs)
    {
        throw new NotImplementedException();
    }

    public void CopyTLAS(CopyTLASAttribs attribs)
    {
        throw new NotImplementedException();
    }

    public void WriteBLASCompactedSize(WriteBLASCompactedSizeAttribs attribs)
    {
        throw new NotImplementedException();
    }

    public void WriteTLASCompactedSize(WriteTLASCompactedSizeAttribs attribs)
    {
        throw new NotImplementedException();
    }

    public void TraceRays(TraceRaysAttribs attribs)
    {
        throw new NotImplementedException();
    }

    public void TraceRaysIndirect(TraceRaysIndirectAttribs attribs)
    {
        throw new NotImplementedException();
    }

    public void UpdateSBT(IShaderBindingTable sbt, UpdateIndirectRTBufferAttribs attribs)
    {
        throw new NotImplementedException();
    }

    public void BeginDebugGroup(string name)
    {
        throw new NotImplementedException();
    }

    public unsafe void BeginDebugGroup(string name, float* color)
    {
        throw new NotImplementedException();
    }

    public void BeginDebugGroup(string name, float[] color)
    {
        throw new NotImplementedException();
    }

    public void EndDebugGroup()
    {
        throw new NotImplementedException();
    }

    public void InsertDebugLabel(string label)
    {
        throw new NotImplementedException();
    }

    public unsafe void InsertDebugLabel(string label, float* color)
    {
        throw new NotImplementedException();
    }

    public void InsertDebugLabel(string label, float[] color)
    {
        throw new NotImplementedException();
    }

    public ICommandQueue LockCommandQueue()
    {
        throw new NotImplementedException();
    }

    public void UnlockCommandQueue()
    {
        throw new NotImplementedException();
    }

    public void SetShadingRate(ShadingRate baseRate, ShadingRateCombiner primitiveCombiner,
        ShadingRateCombiner textureCombiner)
    {
        throw new NotImplementedException();
    }

    public void BindSparseResourceMemory(BindSparseResourceMemoryAttribs attribs)
    {
        throw new NotImplementedException();
    }

    public void ClearStats()
    {
        throw new NotImplementedException();
    }
}
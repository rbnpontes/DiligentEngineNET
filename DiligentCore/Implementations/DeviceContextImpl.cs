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

    private OptimizedClearValue.__Internal[] _clearValues = [];

    public void BeginRenderPass(BeginRenderPassAttribs attribs)
    {
        if (attribs.ClearValues.Length > 0)
            _clearValues = new OptimizedClearValue.__Internal[attribs.ClearValues.Length];

        var data = BeginRenderPassAttribs.GetInternalStruct(attribs);
        fixed (OptimizedClearValue.__Internal* clearValuesPtr = _clearValues)
        {
            for (var i = 0; i < attribs.ClearValues.Length; ++i)
                clearValuesPtr[i] = OptimizedClearValue.GetInternalStruct(attribs.ClearValues[i]);
            data.pClearValues = new IntPtr(clearValuesPtr);
            Interop.device_context_begin_render_pass(Handle, new IntPtr(&data));
        }
    }

    public void NextSubpass()
    {
        Interop.device_context_next_subpass(Handle);
    }

    public void EndRenderPass()
    {
        Interop.device_context_end_render_pass(Handle);
    }

    public void Draw(DrawAttribs attribs)
    {
        var data = DrawAttribs.GetInternalStruct(attribs);
        Interop.device_context_draw(Handle, new IntPtr(&data));
    }

    public void DrawIndexed(DrawIndexedAttribs attribs)
    {
        var data = DrawIndexedAttribs.GetInternalStruct(attribs);
        Interop.device_context_draw_indexed(Handle, new IntPtr(&data));
    }

    public void DrawIndirect(DrawIndirectAttribs attribs)
    {
        var data = DrawIndirectAttribs.GetInternalStruct(attribs);
        Interop.device_context_draw_indirect(Handle, new IntPtr(&data));
    }

    public void DrawIndexedIndirect(DrawIndexedIndirectAttribs attribs)
    {
        var data = DrawIndexedIndirectAttribs.GetInternalStruct(attribs);
        Interop.device_context_draw_indexed_indirect(Handle, new IntPtr(&data));
    }

    public void DrawMesh(DrawMeshAttribs attribs)
    {
        var data = DrawMeshAttribs.GetInternalStruct(attribs);
        Interop.device_context_draw_mesh(Handle, new IntPtr(&data));
    }

    public void DrawMeshIndirect(DrawMeshIndirectAttribs attribs)
    {
        var data = DrawMeshIndirectAttribs.GetInternalStruct(attribs);
        Interop.device_context_draw_mesh_indirect(Handle, new IntPtr(&data));
    }

    private MultiDrawItem.__Internal[] _tmpMultiDrawItems = [];

    public void MultiDraw(MultiDrawAttribs attribs)
    {
        var data = MultiDrawAttribs.GetInternalStruct(attribs);
        if (attribs.DrawItems.Length > _tmpMultiDrawItems.Length)
            _tmpMultiDrawItems = new MultiDrawItem.__Internal[attribs.DrawItems.Length];

        fixed (MultiDrawItem.__Internal* drawItemsPtr = _tmpMultiDrawItems)
        {
            for (var i = 0; i < attribs.DrawItems.Length; ++i)
                drawItemsPtr[i] = MultiDrawItem.GetInternalStruct(attribs.DrawItems[i]);
            data.pDrawItems = new IntPtr(drawItemsPtr);

            Interop.device_context_multi_draw(Handle, new IntPtr(&data));
        }
    }

    private MultiDrawIndexedItem.__Internal[] _tmpMultiDrawIndexedItems = [];

    public void MultiDrawIndexed(MultiDrawIndexedAttribs attribs)
    {
        var data = MultiDrawIndexedAttribs.GetInternalStruct(attribs);
        if (attribs.DrawItems.Length > _tmpMultiDrawIndexedItems.Length)
            _tmpMultiDrawIndexedItems = new MultiDrawIndexedItem.__Internal[attribs.DrawItems.Length];

        fixed (MultiDrawIndexedItem.__Internal* drawItemsPtr = _tmpMultiDrawIndexedItems)
        {
            for (var i = 0; i < attribs.DrawItems.Length; ++i)
                drawItemsPtr[i] = MultiDrawIndexedItem.GetInternalStruct(attribs.DrawItems[i]);
            data.pDrawItems = new IntPtr(drawItemsPtr);

            Interop.device_context_multi_draw_indexed(Handle, new IntPtr(&data));
        }
    }

    public void DispatchCompute(DispatchComputeAttribs attribs)
    {
        var data = DispatchComputeAttribs.GetInternalStruct(attribs);
        Interop.device_context_dispatch_compute(Handle, new IntPtr(&data));
    }

    public void DispatchComputeIndirect(DispatchComputeIndirectAttribs attribs)
    {
        var data = DispatchComputeIndirectAttribs.GetInternalStruct(attribs);
        Interop.device_context_dispatch_compute_indirect(Handle, new IntPtr(&data));
    }

    public void DispatchTile(DispatchTileAttribs attribs)
    {
        var data = DispatchTileAttribs.GetInternalStruct(attribs);
        Interop.device_context_dispatch_tile(Handle, new IntPtr(&data));
    }

    public void GetTileSize(ref uint tileSizeX, ref uint tileSizeY)
    {
        fixed (uint* tileSizeXPtr = &tileSizeX)
        fixed (uint* tileSizeYPtr = &tileSizeY)
            Interop.device_context_get_tile_size(Handle, new IntPtr(tileSizeXPtr), new IntPtr(tileSizeYPtr));
    }

    public void ClearDepthStencil(ITextureView view, ClearDepthStencilFlags clearFlags, float depth, byte stencil,
        ResourceStateTransitionMode stateTransitionMode)
    {
        Interop.device_context_clear_depth_stencil(Handle,
            view.Handle,
            clearFlags,
            depth,
            stencil,
            stateTransitionMode);
    }

    private void ClearRenderTarget(ITextureView view, void* rgba, ResourceStateTransitionMode stateTransitionMode)
    {
        Interop.device_context_clear_render_target(Handle,
            view.Handle,
            new IntPtr(rgba),
            stateTransitionMode);
    }

    public void ClearRenderTarget(ITextureView view, float[] rgba, ResourceStateTransitionMode stateTransitionMode)
    {
        fixed (void* rgbaPtr = rgba)
            ClearRenderTarget(view, rgbaPtr, stateTransitionMode);
    }

    public void ClearRenderTarget(ITextureView view, uint[] rgba, ResourceStateTransitionMode stateTransitionMode)
    {
        fixed (void* rgbaPtr = rgba)
            ClearRenderTarget(view, rgbaPtr, stateTransitionMode);
    }

    public void ClearRenderTarget(ITextureView view, int[] rgba, ResourceStateTransitionMode stateTransitionMode)
    {
        fixed (void* rgbaPtr = rgba)
            ClearRenderTarget(view, rgbaPtr, stateTransitionMode);
    }

    public ICommandList FinishCommandList()
    {
        var commandListPtr = IntPtr.Zero;
        Interop.device_context_finish_command_list(Handle, new IntPtr(&commandListPtr));

        return DiligentObjectsFactory.CreateCommandList(commandListPtr);
    }

    public void ExecuteCommandLists(ICommandList[] commandLists)
    {
        if (commandLists.Length > _tmpPointersBuffer.Length)
            _tmpPointersBuffer = new IntPtr[commandLists.Length];

        fixed (nint* commandListsPtr = _tmpPointersBuffer)
            Interop.device_context_finish_command_list(Handle, new IntPtr(commandListsPtr));

        foreach (var commandList in commandLists)
        {
            NativeObjectRegistry.RemoveObject(commandList.Handle);
            commandList.Dispose();
        }
    }

    public void EnqueueSignal(IFence fence, ulong value)
    {
        Interop.device_context_enqueue_signal(Handle, fence.Handle, value);
    }

    public void DeviceWaitForFence(IFence fence, ulong value)
    {
        Interop.device_context_device_wait_for_fence(Handle, fence.Handle, value);
    }

    public void WaitForIdle()
    {
        Interop.device_context_wait_for_idle(Handle);
    }

    public void BeginQuery(IQuery query)
    {
        Interop.device_context_begin_query(Handle, query.Handle);
    }

    public void EndQuery(IQuery query)
    {
        Interop.device_context_end_query(Handle, query.Handle);
    }

    public void Flush()
    {
        Interop.device_context_flush(Handle);
    }

    public void CopyBuffer(IBuffer srcBuffer, ulong srcOffset, ResourceStateTransitionMode srcBufferTransitionMode,
        IBuffer dstBuffer, ulong dstOffset, ulong size, ResourceStateTransitionMode dstBufferTransitionMode)
    {
        Interop.device_context_copy_buffer(Handle,
            srcBuffer.Handle,
            srcOffset,
            srcBufferTransitionMode,
            dstBuffer.Handle,
            dstOffset,
            size,
            dstBufferTransitionMode);
    }

    public IntPtr MapBuffer(IBuffer buffer, MapType mapType, MapFlags mapFlags)
    {
        var ptr = IntPtr.Zero;
        Interop.device_context_map_buffer(Handle, buffer.Handle, mapType, mapFlags, new IntPtr(&ptr));
        return ptr;
    }

    public void UnmapBuffer(IBuffer buffer, MapType mapType)
    {
        Interop.device_context_unmap_buffer(Handle, buffer.Handle, mapType);
    }

    public void UpdateTexture(ITexture texture, uint mipLevel, uint slice, Box dstBox, TextureSubResData subResData,
        ResourceStateTransitionMode srcBufferTransitionMode, ResourceStateTransitionMode textureTransitionMode)
    {
        var boxData = Box.GetInternalStruct(dstBox);
        var subRes = TextureSubResData.GetInternalStruct(subResData);

        Interop.device_context_update_texture(Handle,
            texture.Handle,
            mipLevel,
            slice,
            new IntPtr(&boxData),
            new IntPtr(&subRes),
            srcBufferTransitionMode,
            textureTransitionMode);
    }

    public void CopyTexture(CopyTextureAttribs copyAttribs)
    {
        var srcBoxData = Box.GetInternalStruct(copyAttribs.SrcBox ?? new Box());
        var copyAttribsData = CopyTextureAttribs.GetInternalStruct(copyAttribs);
        copyAttribsData.pSrcBox = copyAttribs.SrcBox is null ? IntPtr.Zero : new IntPtr(&srcBoxData);

        Interop.device_context_copy_texture(Handle, new IntPtr(&copyAttribsData));
    }

    public void MapTextureSubresource(ITexture texture, uint mipLevel, uint arraySlice, MapType mapType,
        MapFlags mapFlags,
        Box? mapRegion, MappedTextureSubresource mappedData)
    {
        var mapRegionData = Box.GetInternalStruct(mapRegion ?? new Box());
        var mappedTexSubRes = MappedTextureSubresource.GetInternalStruct(mappedData);

        Interop.device_context_map_texture_subresource(Handle,
            texture.Handle,
            mipLevel,
            arraySlice,
            mapType,
            mapFlags,
            new IntPtr(&mapRegionData),
            new IntPtr(&mappedTexSubRes));
    }

    public void GenerateMips(ITextureView textureView)
    {
        Interop.device_context_generate_mips(Handle, textureView.Handle);
    }

    public void FinishFrame()
    {
        Interop.device_context_finish_frame(Handle);
    }

    private StateTransitionDesc.__Internal[] _tmpResourceBarriers = [];

    public void TransitionResourceStates(StateTransitionDesc[] resourceBarriers)
    {
        if (resourceBarriers.Length > _tmpResourceBarriers.Length)
            _tmpResourceBarriers = new StateTransitionDesc.__Internal[resourceBarriers.Length];

        fixed (StateTransitionDesc.__Internal* resourceBarriersPtr = _tmpResourceBarriers)
        {
            for (var i = 0; i < resourceBarriers.Length; ++i)
                resourceBarriersPtr[i] = StateTransitionDesc.GetInternalStruct(resourceBarriers[i]);

            Interop.device_context_transition_resource_states(Handle,
                (uint)resourceBarriers.Length,
                new IntPtr(resourceBarriersPtr));
        }
    }

    public void ResolveTextureSubresource(ITexture srcTexture, ITexture dstTexture,
        ResolveTextureSubresourceAttribs resolveAttribs)
    {
        var data = ResolveTextureSubresourceAttribs.GetInternalStruct(resolveAttribs);
        Interop.device_context_resolve_texture_subresource(Handle,
            srcTexture.Handle,
            dstTexture.Handle,
            new IntPtr(&data));
    }

    public void BuildBLAS(BuildBLASAttribs attribs)
    {
        using var strAlloc = new StringAllocator();
        var data = BuildBLASAttribs.GetInternalStruct(attribs);
        var triangleData = attribs.TriangleData
            .Select(x =>
            {
                var result = BLASBuildTriangleData.GetInternalStruct(x);
                result.GeometryName = strAlloc.Acquire(x.GeometryName);
                return result;
            }).ToArray();
        var boxData = attribs.BoxData
            .Select(x =>
            {
                var result = BLASBuildBoundingBoxData.GetInternalStruct(x);
                result.GeometryName = strAlloc.Acquire(x.GeometryName);
                return result;
            }).ToArray();
        Interop.device_context_build_blas(Handle,
            new IntPtr(&data));
    }

    public void BuildTLAS(BuildTLASAttribs attribs)
    {
        using var strAlloc = new StringAllocator();
        var data = BuildTLASAttribs.GetInternalStruct(attribs);
        var instances = attribs.Instances
            .Select(x =>
            {
                var result = TLASBuildInstanceData.GetInternalStruct(x);
                result.InstanceName = strAlloc.Acquire(x.InstanceName);
                return result;
            })
            .ToArray();

        fixed (void* instancesPtr = instances)
        {
            data.pInstances = new IntPtr(instancesPtr);
            Interop.device_context_build_tlas(Handle, new IntPtr(&data));
        }
    }

    public void CopyBLAS(CopyBLASAttribs attribs)
    {
        var data = CopyBLASAttribs.GetInternalStruct(attribs);
        Interop.device_context_copy_blas(Handle,
            new IntPtr(&data));
    }

    public void CopyTLAS(CopyTLASAttribs attribs)
    {
        var data = CopyTLASAttribs.GetInternalStruct(attribs);
        Interop.device_context_copy_tlas(Handle, new IntPtr(&data));
    }

    public void WriteBLASCompactedSize(WriteBLASCompactedSizeAttribs attribs)
    {
        var data = WriteBLASCompactedSizeAttribs.GetInternalStruct(attribs);
        Interop.device_context_write_blascompacted_size(Handle,
            new IntPtr(&data));
    }

    public void WriteTLASCompactedSize(WriteTLASCompactedSizeAttribs attribs)
    {
        var data = WriteTLASCompactedSizeAttribs.GetInternalStruct(attribs);
        Interop.device_context_write_tlascompacted_size(Handle, new IntPtr(&data));
    }

    public void TraceRays(TraceRaysAttribs attribs)
    {
        var data = TraceRaysAttribs.GetInternalStruct(attribs);
        Interop.device_context_trace_rays(Handle, new IntPtr(&data));
    }

    public void TraceRaysIndirect(TraceRaysIndirectAttribs attribs)
    {
        var data = TraceRaysIndirectAttribs.GetInternalStruct(attribs);
        Interop.device_context_trace_rays_indirect(Handle, new IntPtr(&data));
    }

    public void UpdateSBT(IShaderBindingTable sbt, UpdateIndirectRTBufferAttribs attribs)
    {
        var data = UpdateIndirectRTBufferAttribs.GetInternalStruct(attribs);
        Interop.device_context_update_sbt(Handle, sbt.Handle, new IntPtr(&data));
    }

    public void BeginDebugGroup(string name)
    {
        BeginDebugGroup(name, [0, 0, 0, 1.0f]);
    }

    public unsafe void BeginDebugGroup(string name, float* color)
    {
        using var strAlloc = new StringAllocator();
        Interop.device_context_begin_debug_group(Handle,
            strAlloc.Acquire(name),
            new IntPtr(color));
    }

    public void BeginDebugGroup(string name, float[] color)
    {
        fixed(float* colorPtr = color)
            BeginDebugGroup(name, colorPtr);
    }

    public void EndDebugGroup()
    {
        Interop.device_context_end_debug_group(Handle);
    }

    public void InsertDebugLabel(string label)
    {
        InsertDebugLabel(label, [0, 0, 0, 1.0f]);
    }

    public unsafe void InsertDebugLabel(string label, float* color)
    {
        using var strAlloc = new StringAllocator();
        Interop.device_context_insert_debug_label(Handle,
            strAlloc.Acquire(label),
            new IntPtr(color));
    }

    public void InsertDebugLabel(string label, float[] color)
    {
        fixed(float* colorPtr = color)
            InsertDebugLabel(label, colorPtr);
    }

    public ICommandQueue LockCommandQueue()
    {
        var ptr = Interop.device_context_lock_command_queue(Handle);
        return DiligentObjectsFactory.CreateCommandQueue(ptr);
    }

    public void UnlockCommandQueue()
    {
        Interop.device_context_unlock_command_queue(Handle);
    }

    public void SetShadingRate(ShadingRate baseRate, ShadingRateCombiner primitiveCombiner,
        ShadingRateCombiner textureCombiner)
    {
        Interop.device_context_set_shading_rate(Handle,
            baseRate,
            primitiveCombiner,
            textureCombiner);
    }

    public void BindSparseResourceMemory(BindSparseResourceMemoryAttribs attribs)
    {
        var data = DiligentDescFactory.GetBindSparseResourceMemoryAttribsBytes(attribs);
        fixed(void* dataPtr = data)
            Interop.device_context_bind_sparse_resource_memory(Handle, new IntPtr(&dataPtr));
    }

    public void ClearStats()
    {
        Interop.device_context_clear_stats(Handle);
    }
}
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Diligent.Utils;

public static unsafe class DiligentDescFactory
{
    public static DeviceObjectAttribs GetDeviceObjectAttribs(IntPtr handle)
    {
        var desc = (DeviceObjectAttribs.__Internal*)handle;
        var result = DeviceObjectAttribs.FromInternalStruct(*desc);
        result.Name = Marshal.PtrToStringAnsi(desc->Name) ?? string.Empty;
        return result;
    }

    public static PipelineStateDesc GetPipelineStateDesc(IntPtr handle)
    {
        var desc = (PipelineStateDesc.__Internal*)handle;
        var result = PipelineStateDesc.FromInternalStruct(*desc);
        result.Name = Marshal.PtrToStringAnsi(desc->Name) ?? string.Empty;

        var variables =
            new ReadOnlySpan<ShaderResourceVariableDesc.__Internal>(desc->ResourceLayout.Variables.ToPointer(),
                (int)desc->ResourceLayout.NumVariables);
        var immutableSamplers = new ReadOnlySpan<ImmutableSamplerDesc.__Internal>(
            desc->ResourceLayout.ImmutableSamplers.ToPointer(), (int)desc->ResourceLayout.NumImmutableSamplers);

        var resourceLayout = result.ResourceLayout;
        resourceLayout.Variables = variables.ToArray().Select(x =>
        {
            var varResult = ShaderResourceVariableDesc.FromInternalStruct(x);
            varResult.Name = Marshal.PtrToStringAnsi(x.Name) ?? string.Empty;
            return varResult;
        }).ToArray();
        resourceLayout.ImmutableSamplers = immutableSamplers.ToArray().Select(x =>
        {
            var samplerResult = ImmutableSamplerDesc.FromInternalStruct(x);
            var samplerDesc = samplerResult.Desc;
            samplerDesc.Name = Marshal.PtrToStringAnsi(x.Desc.Name) ?? string.Empty;
            samplerResult.SamplerOrTextureName = Marshal.PtrToStringAnsi(x.SamplerOrTextureName) ?? string.Empty;
            samplerResult.Desc = samplerDesc;
            return samplerResult;
        }).ToArray();

        result.ResourceLayout = resourceLayout;
        return result;
    }

    public static GraphicsPipelineDesc GetGraphicsPipelineDesc(IntPtr handle)
    {
        var data = (GraphicsPipelineDesc.__Internal*)handle;
        var desc = GraphicsPipelineDesc.FromInternalStruct(*data);

        var layoutElements = new ReadOnlySpan<LayoutElement.__Internal>(data->InputLayout.LayoutElements.ToPointer(),
            (int)data->InputLayout.NumElements);

        var inputLayoutDesc = desc.InputLayout;
        inputLayoutDesc.LayoutElements = layoutElements.ToArray().Select(x =>
        {
            var layoutElement = LayoutElement.FromInternalStruct(x);
            var hlslSemantic = Marshal.PtrToStringAnsi(x.HLSLSemantic);
            if (!string.IsNullOrEmpty(hlslSemantic))
                layoutElement.HLSLSemantic = hlslSemantic;
            return layoutElement;
        }).ToArray();

        desc.InputLayout = inputLayoutDesc;
        return desc;
    }

    public static RayTracingPipelineDesc GetRayTracingPipelineDesc(IntPtr handle)
    {
        var data = (RayTracingPipelineDesc.__Internal*)handle;
        return RayTracingPipelineDesc.FromInternalStruct(*data);
    }

    public static TilePipelineDesc GetTilePipelineDesc(IntPtr handle)
    {
        var data = (TilePipelineDesc.__Internal*)handle;
        return TilePipelineDesc.FromInternalStruct(*data);
    }

    public static FenceDesc GetFenceDesc(IntPtr handle)
    {
        var data = (FenceDesc.__Internal*)Fence.Interop.fence_get_desc(handle);
        var result = FenceDesc.FromInternalStruct(*data);
        result.Name = Marshal.PtrToStringAnsi(data->Name) ?? string.Empty;
        return result;
    }

    public static QueryDesc GetQueryDesc(IntPtr handle)
    {
        var data = (QueryDesc.__Internal*)Query.Interop.query_get_desc(handle);
        var result = QueryDesc.FromInternalStruct(*data);
        result.Name = Marshal.PtrToStringAnsi(data->Name) ?? string.Empty;
        return result;
    }

    public static int GetRenderPassDescSize(RenderPassDesc desc)
    {
        var size = Unsafe.SizeOf<RenderPassDesc.__Internal>()
                   + desc.Attachments.Length * Unsafe.SizeOf<RenderPassAttachmentDesc.__Internal>()
                   + desc.Subpasses.Length * Unsafe.SizeOf<SubpassDesc.__Internal>();
        foreach (var subPass in desc.Subpasses)
        {
            size += subPass.InputAttachments.Length * Unsafe.SizeOf<AttachmentReference.__Internal>();
            size += subPass.RenderTargetAttachments.Length * Unsafe.SizeOf<AttachmentReference.__Internal>();

            if (subPass.ResolveAttachments.Length == subPass.RenderTargetAttachments.Length)
                size += subPass.RenderTargetAttachments.Length * Unsafe.SizeOf<AttachmentReference.__Internal>();

            if (subPass.DepthStencilAttachment is not null)
                size += Unsafe.SizeOf<AttachmentReference.__Internal>();

            if (subPass.ShadingRateAttachment is not null)
                size += Unsafe.SizeOf<ShadingRateAttachment.__Internal>();
        }

        size += desc.Dependencies.Length * Unsafe.SizeOf<SubpassDependencyDesc.__Internal>();
        size += desc.Name.Length + 1;
        return size;
    }

    public static byte[] GetRenderPassDescBytes(RenderPassDesc desc)
    {
        var nameBytes = Encoding.ASCII.GetBytes(desc.Name);
        var structSize = GetRenderPassDescSize(desc);
        var result = new byte[structSize];
        var span = result.AsSpan();

        fixed (void* basePtr = span)
        fixed (void* nameBytesPtr = nameBytes)
        {
            var ptr = new IntPtr(basePtr);
            var namePtr = IntPtr.Add(ptr, structSize - desc.Name.Length - 1);

            Unsafe.CopyBlockUnaligned(namePtr.ToPointer(), nameBytesPtr, (uint)nameBytes.Length);

            var renderPassDescPtr = (RenderPassDesc.__Internal*)ptr;
            *renderPassDescPtr = RenderPassDesc.GetInternalStruct(desc);
            renderPassDescPtr->Name = namePtr;

            ptr = IntPtr.Add(ptr, Unsafe.SizeOf<RenderPassDesc.__Internal>());
            ptr = CollectAttachments(ptr, desc);
            ptr = CollectSubPasses(ptr, renderPassDescPtr, desc);
            CollectSubPassDependencies(ptr, renderPassDescPtr, desc);
        }

        return result;

        IntPtr CollectAttachments(IntPtr ptr, RenderPassDesc renderPassDesc)
        {
            var renderPassDescPtr =
                (RenderPassDesc.__Internal*)IntPtr.Subtract(ptr, Unsafe.SizeOf<RenderPassDesc.__Internal>());
            renderPassDescPtr->pAttachments = ptr;
            renderPassDescPtr->AttachmentCount = (uint)renderPassDesc.Attachments.Length;

            foreach (var attachment in renderPassDesc.Attachments)
            {
                var attachmentPtr = (RenderPassAttachmentDesc.__Internal*)ptr;
                *attachmentPtr = RenderPassAttachmentDesc.GetInternalStruct(attachment);
                ptr = IntPtr.Add(ptr, Unsafe.SizeOf<RenderPassAttachmentDesc.__Internal>());
            }

            return ptr;
        }

        IntPtr CollectSubPasses(IntPtr ptr, RenderPassDesc.__Internal* renderPassDescPtr, RenderPassDesc renderPassDesc)
        {
            var totalOfInputAttachments = renderPassDesc.Subpasses.Sum(x => x.InputAttachments.Length);
            var totalOfRenderTargetAttachments = renderPassDesc.Subpasses.Sum(x => x.RenderTargetAttachments.Length);
            var totalOfResolveAttachments = renderPassDesc.Subpasses.Sum(x =>
                x.ResolveAttachments.Length == x.RenderTargetAttachments.Length ? x.RenderTargetAttachments.Length : 0);
            var totalOfDepthStencilAttachments =
                renderPassDesc.Subpasses.Sum(x => x.DepthStencilAttachment is null ? 0 : 1);

            var inputAttachmentsPtr =
                IntPtr.Add(ptr, renderPassDesc.Subpasses.Length * Unsafe.SizeOf<SubpassDesc.__Internal>());
            var renderTargetAttachmentsPtr = IntPtr.Add(inputAttachmentsPtr,
                totalOfInputAttachments * Unsafe.SizeOf<AttachmentReference.__Internal>());
            var resolveAttachmentsPtr = IntPtr.Add(renderTargetAttachmentsPtr,
                totalOfRenderTargetAttachments * Unsafe.SizeOf<AttachmentReference.__Internal>());
            var depthStencilAttachmentsPtr = IntPtr.Add(resolveAttachmentsPtr,
                totalOfResolveAttachments * Unsafe.SizeOf<AttachmentReference.__Internal>());
            var shadingRateAttachmentsPtr = IntPtr.Add(depthStencilAttachmentsPtr,
                totalOfDepthStencilAttachments * Unsafe.SizeOf<AttachmentReference.__Internal>());

            renderPassDescPtr->pSubpasses = new IntPtr(ptr);
            renderPassDescPtr->SubpassCount = (uint)renderPassDesc.Subpasses.Length;
            foreach (var subPass in renderPassDesc.Subpasses)
            {
                var subPassPtr = (SubpassDesc.__Internal*)ptr;
                *subPassPtr = SubpassDesc.GetInternalStruct(subPass);
                subPassPtr->pInputAttachments =
                    subPassPtr->pRenderTargetAttachments =
                        subPassPtr->pResolveAttachments =
                            subPassPtr->pDepthStencilAttachment =
                                subPassPtr->pShadingRateAttachment = IntPtr.Zero;

                if (subPass.InputAttachments.Length != 0)
                {
                    subPassPtr->pInputAttachments = inputAttachmentsPtr;
                    subPassPtr->InputAttachmentCount = (uint)subPass.InputAttachments.Length;
                    foreach (var inputAttachment in subPass.InputAttachments)
                    {
                        *((AttachmentReference.__Internal*)inputAttachmentsPtr) =
                            AttachmentReference.GetInternalStruct(inputAttachment);
                        inputAttachmentsPtr = IntPtr.Add(inputAttachmentsPtr,
                            Unsafe.SizeOf<AttachmentReference.__Internal>());
                    }
                }

                if (subPass.RenderTargetAttachments.Length != 0)
                {
                    subPassPtr->pRenderTargetAttachments = renderTargetAttachmentsPtr;
                    subPassPtr->RenderTargetAttachmentCount = (uint)subPass.RenderTargetAttachments.Length;
                    foreach (var renderTargetAttachment in subPass.RenderTargetAttachments)
                    {
                        *((AttachmentReference.__Internal*)renderTargetAttachmentsPtr) =
                            AttachmentReference.GetInternalStruct(renderTargetAttachment);
                        renderTargetAttachmentsPtr = IntPtr.Add(renderTargetAttachmentsPtr,
                            Unsafe.SizeOf<AttachmentReference.__Internal>());
                    }
                }

                if (subPass.ResolveAttachments.Length == subPass.RenderTargetAttachments.Length)
                {
                    subPassPtr->pResolveAttachments = resolveAttachmentsPtr;
                    foreach (var resolveAttachment in subPass.ResolveAttachments)
                    {
                        *((AttachmentReference.__Internal*)resolveAttachmentsPtr) =
                            AttachmentReference.GetInternalStruct(resolveAttachment);
                        resolveAttachmentsPtr = IntPtr.Add(resolveAttachmentsPtr,
                            Unsafe.SizeOf<AttachmentReference.__Internal>());
                    }
                }

                if (subPass.DepthStencilAttachment is not null)
                {
                    subPassPtr->pDepthStencilAttachment = depthStencilAttachmentsPtr;
                    *((AttachmentReference.__Internal*)depthStencilAttachmentsPtr) =
                        AttachmentReference.GetInternalStruct(subPass.DepthStencilAttachment);
                    depthStencilAttachmentsPtr = IntPtr.Add(depthStencilAttachmentsPtr,
                        Unsafe.SizeOf<DepthStencilStateDesc.__Internal>());
                }

                if (subPass.ShadingRateAttachment is not null)
                {
                    subPassPtr->pShadingRateAttachment = shadingRateAttachmentsPtr;
                    *((ShadingRateAttachment.__Internal*)shadingRateAttachmentsPtr) =
                        ShadingRateAttachment.GetInternalStruct(subPass.ShadingRateAttachment);
                    shadingRateAttachmentsPtr = IntPtr.Add(shadingRateAttachmentsPtr,
                        Unsafe.SizeOf<ShadingRateAttachment.__Internal>());
                }

                ptr = IntPtr.Add(ptr, Unsafe.SizeOf<SubpassDesc>());
            }

            return shadingRateAttachmentsPtr;
        }

        void CollectSubPassDependencies(IntPtr ptr, RenderPassDesc.__Internal* renderPassDescPtr,
            RenderPassDesc renderPassDesc)
        {
            renderPassDescPtr->pDependencies = ptr;
            renderPassDescPtr->DependencyCount = (uint)renderPassDesc.Dependencies.Length;

            foreach (var dep in renderPassDesc.Dependencies)
            {
                var depPtr = (SubpassDependencyDesc.__Internal*)ptr;
                *depPtr = SubpassDependencyDesc.GetInternalStruct(dep);
                ptr = IntPtr.Add(ptr, Unsafe.SizeOf<SubpassDependencyDesc.__Internal>());
            }
        }
    }

    public static RenderPassDesc GetRenderPassDesc(IntPtr handle)
    {
        var data = (RenderPassDesc.__Internal*)handle;
        var result = RenderPassDesc.FromInternalStruct(*data);
        result.Name = Marshal.PtrToStringAnsi(data->Name) ?? string.Empty;

        var attachmentsData =
            new ReadOnlySpan<RenderPassAttachmentDesc.__Internal>(data->pAttachments.ToPointer(),
                (int)data->AttachmentCount);
        var subPassesData =
            new ReadOnlySpan<SubpassDesc.__Internal>(data->pSubpasses.ToPointer(), (int)data->SubpassCount);
        var subDependenciesData =
            new ReadOnlySpan<SubpassDependencyDesc.__Internal>(data->pDependencies.ToPointer(),
                (int)data->DependencyCount);

        result.Attachments = attachmentsData.ToArray()
            .Select(RenderPassAttachmentDesc.FromInternalStruct)
            .ToArray();
        result.Subpasses = subPassesData.ToArray()
            .Select(subPassData =>
            {
                var subPass = SubpassDesc.FromInternalStruct(subPassData);
                var inputAttachments = new ReadOnlySpan<AttachmentReference.__Internal>(
                    subPassData.pInputAttachments.ToPointer(),
                    (int)subPassData.InputAttachmentCount);
                var renderTargetAttachments = new ReadOnlySpan<AttachmentReference.__Internal>(
                    subPassData.pRenderTargetAttachments.ToPointer(),
                    (int)subPassData.RenderTargetAttachmentCount);
                var resolveAttachments = new ReadOnlySpan<AttachmentReference.__Internal>(
                    subPassData.pResolveAttachments.ToPointer(),
                    subPassData.pResolveAttachments != IntPtr.Zero ? (int)subPassData.RenderTargetAttachmentCount : 0);

                subPass.InputAttachments = inputAttachments.ToArray()
                    .Select(AttachmentReference.FromInternalStruct)
                    .ToArray();
                subPass.RenderTargetAttachments = renderTargetAttachments.ToArray()
                    .Select(AttachmentReference.FromInternalStruct)
                    .ToArray();
                subPass.ResolveAttachments = resolveAttachments.ToArray()
                    .Select(AttachmentReference.FromInternalStruct)
                    .ToArray();

                if (subPassData.pDepthStencilAttachment != IntPtr.Zero)
                    subPass.DepthStencilAttachment =
                        AttachmentReference.FromInternalStruct(
                            *(AttachmentReference.__Internal*)subPassData.pDepthStencilAttachment);
                if (subPassData.pShadingRateAttachment != IntPtr.Zero)
                    subPass.ShadingRateAttachment =
                        ShadingRateAttachment.FromInternalStruct(
                            *(ShadingRateAttachment.__Internal*)subPassData.pShadingRateAttachment);

                return subPass;
            })
            .ToArray();
        result.Dependencies = subDependenciesData.ToArray()
            .Select(SubpassDependencyDesc.FromInternalStruct)
            .ToArray();
        return result;
    }

    public static FramebufferDesc GetFramebufferDesc(IntPtr handle)
    {
        var data = (FramebufferDesc.__Internal*)handle;
        var result = FramebufferDesc.FromInternalStruct(*data);
        var attachments = new ReadOnlySpan<IntPtr>(data->ppAttachments.ToPointer(), (int)data->AttachmentCount);

        result.Name = Marshal.PtrToStringAnsi(data->Name) ?? string.Empty;
        result.Attachments = attachments.ToArray().Select(x =>
        {
            return NativeObjectRegistry.GetOrCreate<ITextureView>(() => new UnDisposableTextureView(x), x);
        }).ToArray();
        return result;
    }

    public static ScratchBufferSizes GetStratchBufferSizes(IntPtr handle)
    {
        var data = (ScratchBufferSizes.__Internal*)handle;
        return ScratchBufferSizes.FromInternalStruct(*data);
    }

    public static BottomLevelASDesc GetBottomLevelASDesc(IntPtr handle)
    {
        var data = (BottomLevelASDesc.__Internal*)handle;
        var result = BottomLevelASDesc.FromInternalStruct(*data);

        var triangles = new ReadOnlySpan<BLASTriangleDesc.__Internal>(data->pTriangles.ToPointer(),
            (int)data->TriangleCount);
        var boxes = new ReadOnlySpan<BLASBoundingBoxDesc.__Internal>(data->pBoxes.ToPointer(),
            (int)data->BoxCount);

        result.Name = Marshal.PtrToStringAnsi(data->Name) ?? string.Empty;
        result.Triangles = triangles.ToArray()
            .Select(x =>
            {
                var res = BLASTriangleDesc.FromInternalStruct(x);
                res.GeometryName = Marshal.PtrToStringAnsi(x.GeometryName) ?? string.Empty;
                return res;
            }).ToArray();
        result.Boxes = boxes.ToArray()
            .Select(x =>
            {
                var res = BLASBoundingBoxDesc.FromInternalStruct(x);
                res.GeometryName = Marshal.PtrToStringAnsi(x.GeometryName) ?? string.Empty;
                return res;
            }).ToArray();

        return result;
    }

    public static TopLevelASDesc GetTopLevelASDesc(IntPtr handle)
    {
        var data = (TopLevelASDesc.__Internal*)handle;
        var result = TopLevelASDesc.FromInternalStruct(*data);

        result.Name = Marshal.PtrToStringAnsi(data->Name) ?? string.Empty;
        return result;
    }

    public static TLASInstanceDesc GetTLASInstanceDesc(IntPtr handle)
    {
        var data = (TLASInstanceDesc.__Internal*)handle;
        return TLASInstanceDesc.FromInternalStruct(*data);
    }

    public static TLASBuildInfo GetTLASBuildInfo(IntPtr handle)
    {
        var data = (TLASBuildInfo.__Internal*)handle;
        return TLASBuildInfo.FromInternalStruct(*data);
    }

    public static ShaderBindingTableDesc GetShaderBindingTableDesc(IntPtr handle)
    {
        var data = (ShaderBindingTableDesc.__Internal*)handle;
        var result = ShaderBindingTableDesc.FromInternalStruct(*data);
        result.Name = Marshal.PtrToStringAnsi(data->Name) ?? string.Empty;
        return result;
    }

    public static PipelineResourceSignatureDesc GetPipelineResourceSignatureDesc(IntPtr handle)
    {
        var data = (PipelineResourceSignatureDesc.__Internal*)handle;
        var result = PipelineResourceSignatureDesc.FromInternalStruct(*data);

        var resources =
            new ReadOnlySpan<PipelineResourceDesc.__Internal>(data->Resources.ToPointer(), (int)data->NumResources);
        var immutableSamplers = new ReadOnlySpan<ImmutableSamplerDesc.__Internal>(data->ImmutableSamplers.ToPointer(),
            (int)data->NumImmutableSamplers);

        var combinedSamplerSuffix = Marshal.PtrToStringAnsi(data->CombinedSamplerSuffix);
        result.Name = Marshal.PtrToStringAnsi(data->Name) ?? string.Empty;
        result.CombinedSamplerSuffix = combinedSamplerSuffix ?? result.CombinedSamplerSuffix;
        result.Resources = resources.ToArray()
            .Select(resource =>
            {
                var res = PipelineResourceDesc.FromInternalStruct(resource);
                res.Name = Marshal.PtrToStringAnsi(resource.Name) ?? string.Empty;
                return res;
            }).ToArray();
        result.ImmutableSamplers = immutableSamplers.ToArray()
            .Select(smpl =>
            {
                var immutableSampler = ImmutableSamplerDesc.FromInternalStruct(smpl);
                immutableSampler.SamplerOrTextureName =
                    Marshal.PtrToStringAnsi(smpl.SamplerOrTextureName) ?? string.Empty;
                var sampler = immutableSampler.Desc;
                sampler.Name = Marshal.PtrToStringAnsi(smpl.Desc.Name) ?? string.Empty;
                immutableSampler.Desc = sampler;
                return immutableSampler;
            }).ToArray();
        return result;
    }

    public static ShaderResourceDesc GetShaderResourceDesc(IntPtr handle)
    {
        var data = (ShaderResourceDesc.__Internal*)handle;
        var result = ShaderResourceDesc.FromInternalStruct(*data);

        result.Name = Marshal.PtrToStringAnsi(data->Name) ?? string.Empty;
        return result;
    }

    public static DeviceMemoryDesc GetDeviceMemoryDesc(IntPtr handle)
    {
        var data = (DeviceMemoryDesc.__Internal*)handle;
        var result = DeviceMemoryDesc.FromInternalStruct(*data);
        result.Name = Marshal.PtrToStringAnsi(data->Name) ?? string.Empty;
        return result;
    }

    public static TextureFormatInfo GetTextureFormatInfo(IntPtr handle)
    {
        var data = (TextureFormatInfo.__Internal*)handle;
        var result = TextureFormatInfo.FromInternalStruct(*data);
        result.Name = Marshal.PtrToStringAnsi(data->Name) ?? string.Empty;
        return result;
    }

    public static TextureFormatInfoExt GetTextureFormatInfoExt(IntPtr handle)
    {
        var data = (TextureFormatInfoExt.__Internal*)handle;
        var result = TextureFormatInfoExt.FromInternalStruct(*data);
        result.Name = Marshal.PtrToStringAnsi(data->Name) ?? string.Empty;
        return result;
    }

    public static SparseTextureFormatInfo GetSparseTextureFormatInfo(IntPtr handle)
    {
        var data = (SparseTextureFormatInfo.__Internal*)handle;
        return SparseTextureFormatInfo.FromInternalStruct(*data);
    }

    public static DeviceContextDesc GetDeviceContextDesc(IntPtr handle)
    {
        var data = (DeviceContextDesc.__Internal*)handle;
        var result = DeviceContextDesc.FromInternalStruct(*data);
        result.Name = Marshal.PtrToStringAnsi(data->Name) ?? string.Empty;
        return result;
    }

    public static DeviceContextStats GetDeviceContextStats(IntPtr handle)
    {
        var data = (DeviceContextStats.__Internal*)handle;
        return DeviceContextStats.FromInternalStruct(*data);
    }

    public static int GetBindSparseResourceMemoryAttribsSize(BindSparseResourceMemoryAttribs value)
    {
        return Unsafe.SizeOf<BindSparseResourceMemoryAttribs.__Internal>()
               + Unsafe.SizeOf<IntPtr>() * value.WaitFences.Length
               + Unsafe.SizeOf<IntPtr>() * value.SignalFences.Length
               + Unsafe.SizeOf<ulong>() * value.WaitFences.Length
               + Unsafe.SizeOf<ulong>() * value.SignalFences.Length
               + Unsafe.SizeOf<SparseBufferMemoryBindInfo.__Internal>() * value.BufferBinds.Length
               + Unsafe.SizeOf<SparseTextureMemoryBindInfo.__Internal>() * value.TextureBinds.Length
               + value.BufferBinds.Sum(bufferBind =>
                   Unsafe.SizeOf<SparseBufferMemoryBindRange.__Internal>() * bufferBind.Ranges.Length)
               + value.TextureBinds.Sum(textureBind =>
                   Unsafe.SizeOf<SparseTextureMemoryBindRange.__Internal>() * textureBind.Ranges.Length);
    }

    public static byte[] GetBindSparseResourceMemoryAttribsBytes(BindSparseResourceMemoryAttribs value)
    {
        var result = new byte[GetBindSparseResourceMemoryAttribsSize(value)];
        fixed (byte* buffer = result)
        {
            var attribs = (BindSparseResourceMemoryAttribs.__Internal*)buffer;
            *attribs = BindSparseResourceMemoryAttribs.GetInternalStruct(value);

            var bufferBindsPtr = buffer + Unsafe.SizeOf<BindSparseResourceMemoryAttribs.__Internal>();
            var textureBindsPtr = bufferBindsPtr +
                                  Unsafe.SizeOf<SparseBufferMemoryBindInfo.__Internal>() * value.BufferBinds.Length;
            var bufferRangesPtr = textureBindsPtr +
                                  Unsafe.SizeOf<SparseTextureMemoryBindInfo.__Internal>() * value.TextureBinds.Length;
            var textureRangesPtr = bufferRangesPtr +
                                   value.BufferBinds.Sum(x => x.Ranges.Length) *
                                   Unsafe.SizeOf<SparseBufferMemoryBindRange.__Internal>();
            var waitFenceValuesPtr = textureRangesPtr +
                                     value.TextureBinds.Sum(x => x.Ranges.Length) *
                                     Unsafe.SizeOf<SparseTextureMemoryBindRange.__Internal>();
            var fencesPtr = waitFenceValuesPtr
                            + (Unsafe.SizeOf<ulong>() * value.WaitFenceValues.Length)
                            + (Unsafe.SizeOf<ulong>() * value.SignalFenceValues.Length);

            attribs->pBufferBinds = new IntPtr(bufferBindsPtr);
            attribs->pTextureBinds = new IntPtr(textureBindsPtr);

            CollectBufferBinds(bufferBindsPtr, bufferRangesPtr);
            CollectTextureBinds(textureBindsPtr, textureRangesPtr);
            CollectWaitFenceValues(attribs, waitFenceValuesPtr);
            CollectFencesPtr(attribs, fencesPtr);
        }

        return result;

        void CollectBufferBinds(byte* bufferBindsPtr, byte* bufferRangesPtr)
        {
            foreach (var bufferBind in value.BufferBinds)
            {
                var bindPtr = (SparseBufferMemoryBindInfo.__Internal*)bufferBindsPtr;
                *bindPtr = SparseBufferMemoryBindInfo.GetInternalStruct(bufferBind);

                bindPtr->pRanges = new IntPtr(bufferRangesPtr);
                foreach (var range in bufferBind.Ranges)
                {
                    var rangePtr = (SparseBufferMemoryBindRange.__Internal*)bufferRangesPtr;
                    *rangePtr = SparseBufferMemoryBindRange.GetInternalStruct(range);

                    bufferRangesPtr += Unsafe.SizeOf<SparseBufferMemoryBindRange.__Internal>();
                }

                bufferBindsPtr += Unsafe.SizeOf<SparseBufferMemoryBindInfo.__Internal>();
            }
        }

        void CollectTextureBinds(byte* textureBindsPtr, byte* textureRangesPtr)
        {
            foreach (var textureBind in value.TextureBinds)
            {
                var bindPtr = (SparseTextureMemoryBindInfo.__Internal*)textureBindsPtr;
                *bindPtr = SparseTextureMemoryBindInfo.GetInternalStruct(textureBind);

                bindPtr->pRanges = new IntPtr(textureRangesPtr);
                foreach (var range in textureBind.Ranges)
                {
                    var rangePtr = (SparseTextureMemoryBindRange.__Internal*)textureRangesPtr;
                    *rangePtr = SparseTextureMemoryBindRange.GetInternalStruct(range);

                    textureRangesPtr += Unsafe.SizeOf<SparseTextureMemoryBindRange.__Internal>();
                }

                textureBindsPtr += Unsafe.SizeOf<SparseTextureMemoryBindInfo.__Internal>();
            }
        }

        void CollectWaitFenceValues(BindSparseResourceMemoryAttribs.__Internal* attribs, byte* fenceValuesPtr)
        {
            attribs->pWaitFenceValues = new IntPtr(fenceValuesPtr);
            for (var i = 0; i < attribs->NumWaitFences; ++i)
            {
                var val = i >= value.WaitFenceValues.Length ? 0 : value.WaitFenceValues[i];
                *((ulong*)fenceValuesPtr) = val;

                fenceValuesPtr += Unsafe.SizeOf<ulong>();
            }

            attribs->pSignalFenceValues = new IntPtr(fenceValuesPtr);
            for (var i = 0; i < attribs->NumSignalFences; ++i)
            {
                var val = i >= value.SignalFenceValues.Length ? 0 : value.SignalFenceValues[i];
                *((ulong*)fenceValuesPtr) = val;

                fenceValuesPtr += Unsafe.SizeOf<ulong>();
            }
        }

        void CollectFencesPtr(BindSparseResourceMemoryAttribs.__Internal* attribs, byte* fencesPtr)
        {
            attribs->ppWaitFences = new IntPtr(fencesPtr);
            foreach (var fence in value.WaitFences)
            {
                var fenceDstPtr = (nint*)fencesPtr;
                *fenceDstPtr = fence.Handle;

                fencesPtr += Unsafe.SizeOf<IntPtr>();
            }

            attribs->ppSignalFences = new IntPtr(fencesPtr);
            foreach (var fence in value.SignalFences)
            {
                var fenceDstPtr = (nint*)fencesPtr;
                *fenceDstPtr = fence.Handle;

                fencesPtr += Unsafe.SizeOf<IntPtr>();
            }
        }
    }

    public static BindSparseResourceMemoryAttribs GetBindSparseResourceMemoryAttribs(IntPtr handle)
    {
        var data = (BindSparseResourceMemoryAttribs.__Internal*)handle;
        var result = BindSparseResourceMemoryAttribs.FromInternalStruct(*data);

        var bufferBinds =
            new ReadOnlySpan<SparseBufferMemoryBindInfo.__Internal>(data->pBufferBinds.ToPointer(),
                    (int)data->NumBufferBinds)
                .ToArray()
                .Select(x =>
                {
                    var bufferBind = SparseBufferMemoryBindInfo.FromInternalStruct(x);
                    var ranges =
                        new ReadOnlySpan<SparseBufferMemoryBindRange.__Internal>(x.pRanges.ToPointer(),
                                (int)x.NumRanges)
                            .ToArray()
                            .Select(SparseBufferMemoryBindRange.FromInternalStruct)
                            .ToArray();
                    bufferBind.Ranges = ranges;
                    return bufferBind;
                })
                .ToArray();
        var textureBinds =
            new ReadOnlySpan<SparseTextureMemoryBindInfo.__Internal>(data->pTextureBinds.ToPointer(),
                    (int)data->NumTextureBinds)
                .ToArray()
                .Select(x =>
                {
                    var textureBind = SparseTextureMemoryBindInfo.FromInternalStruct(x);
                    var ranges =
                        new ReadOnlySpan<SparseTextureMemoryBindRange.__Internal>(x.pRanges.ToPointer(),
                                (int)x.NumRanges)
                            .ToArray()
                            .Select(SparseTextureMemoryBindRange.FromInternalStruct)
                            .ToArray();
                    textureBind.Ranges = ranges;
                    return textureBind;
                })
                .ToArray();
        var waitFences = new ReadOnlySpan<IntPtr>(data->ppWaitFences.ToPointer(), (int)data->NumWaitFences)
            .ToArray()
            .Select(DiligentObjectsFactory.CreateFence)
            .ToArray();
        var waitFenceValues = new ReadOnlySpan<ulong>(data->pWaitFenceValues.ToPointer(), (int)data->NumWaitFences);
        var signalFences = new ReadOnlySpan<IntPtr>(data->ppSignalFences.ToPointer(), (int)data->NumSignalFences)
            .ToArray()
            .Select(DiligentObjectsFactory.CreateFence)
            .ToArray();

        result.BufferBinds = bufferBinds;
        result.TextureBinds = textureBinds;
        result.SignalFences = signalFences;
        result.WaitFences = waitFences;
        result.WaitFenceValues = waitFenceValues.ToArray();
        return result;
    }

    public static ShaderCodeBufferDesc GetShaderCodeBufferDesc(IntPtr handle)
    {
        var data = (ShaderCodeBufferDesc.__Internal*)handle.ToPointer();
        var variables =
            new Span<ShaderCodeVariableDesc.__Internal>(data->pVariables.ToPointer(), (int)data->NumVariables);

        var desc = ShaderCodeBufferDesc.FromInternalStruct(*data);
        desc.Variables = ReadShaderCodeVariableDesc(data->pVariables, (int)data->NumVariables);
        return desc;
        
        ShaderCodeVariableDesc[] ReadShaderCodeVariableDesc(IntPtr data, int num)
        {
            var res = new Span<ShaderCodeVariableDesc.__Internal>(data.ToPointer(), num);
            return res
                .ToArray()
                .Select(x =>
                {
                    var varDesc = ShaderCodeVariableDesc.FromInternalStruct(x);
                    varDesc.Name = Marshal.PtrToStringAnsi(x.Name) ?? string.Empty;
                    varDesc.TypeName = Marshal.PtrToStringAnsi(x.TypeName) ?? string.Empty;
                    varDesc.Members = ReadShaderCodeVariableDesc(x.pMembers, (int)x.NumMembers);
                    return varDesc;
                })
                .ToArray();
        }
    }
}
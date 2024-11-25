using System.Runtime.InteropServices;

namespace Diligent.Utils;

public static unsafe class DiligentDescFactory
{
    public static DeviceObjectAttribs GetDeviceObjectAttribs(IntPtr handle)
    {
        var desc = (DeviceObjectAttribs.__Internal*)DeviceObject.Interop.device_object_get_desc(handle);
        var result = DeviceObjectAttribs.FromInternalStruct(*desc);
        result.Name = Marshal.PtrToStringAnsi(handle) ?? string.Empty;
        return result;
    }

    public static PipelineStateDesc GetPipelineStateDesc(IntPtr handle)
    {
        var desc = (PipelineStateDesc.__Internal*)PipelineState.Interop.pipeline_state_get_desc(handle);
        var result = PipelineStateDesc.FromInternalStruct(*desc);
        result.Name = Marshal.PtrToStringAnsi(handle) ?? string.Empty;

        var variables = new ReadOnlySpan<ShaderResourceVariableDesc.__Internal>(desc->ResourceLayout.Variables.ToPointer(), (int)desc->ResourceLayout.NumVariables);
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
        var data =
            (GraphicsPipelineDesc.__Internal*)PipelineState.Interop.pipeline_state_get_graphics_pipeline_desc(handle);
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
        var data = (RayTracingPipelineDesc.__Internal*)PipelineState.Interop
            .pipeline_state_get_ray_tracing_pipeline_desc(handle);
        return RayTracingPipelineDesc.FromInternalStruct(*data);
    }

    public static TilePipelineDesc GetTilePipelineDesc(IntPtr handle)
    {
        var data = (TilePipelineDesc.__Internal*)PipelineState.Interop.pipeline_state_get_tile_pipeline_desc(handle);
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
}
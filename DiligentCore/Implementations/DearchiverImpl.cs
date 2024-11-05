namespace Diligent;

internal partial class Dearchiver : IDearchiver
{
    public uint ContentVersion => Interop.dearchiver_get_content_version(Handle);

    public Dearchiver() : base()
    {
    }

    internal Dearchiver(IntPtr handle) : base(handle)
    {
    }

    public bool LoadArchive(IDataBlob dataBlob, uint contentVersion = 0, bool makeCopy = false)
    {
        return Interop.dearchiver_load_archive(Handle, dataBlob.Handle, contentVersion, makeCopy);
    }

    public unsafe IShader UnpackShader(ShaderUnpackInfo unpackInfo)
    {
        var shaderPtr = IntPtr.Zero;
        var unpackInfoData = ShaderUnpackInfo.GetInternalStruct(unpackInfo);
        var unpackInfoPtr = &unpackInfoData;

        Interop.dearchiver_unpack_shader(Handle, new IntPtr(unpackInfoPtr), new IntPtr(&shaderPtr));
        if (shaderPtr == IntPtr.Zero)
            throw new NullReferenceException("Failed to unpack shader");
        return NativeObjectRegistry.GetOrCreate<IShader>(() => throw new NotImplementedException(), shaderPtr);
    }

    public unsafe IPipelineState UnpackPipelineState(PipelineStateUnpackInfo unpackInfo)
    {
        var pipelineStatePtr = IntPtr.Zero;
        var unpackInfoData = PipelineStateUnpackInfo.GetInternalStruct(unpackInfo);
        var unpackInfoPtr = &unpackInfoData;

        Interop.dearchiver_unpack_pipeline_state(Handle, new IntPtr(unpackInfoPtr), new IntPtr(&pipelineStatePtr));
        if (pipelineStatePtr == IntPtr.Zero)
            throw new NullReferenceException("Failed to unpack pipeline state");
        return NativeObjectRegistry.GetOrCreate<IPipelineState>(() => throw new NotImplementedException(),
            pipelineStatePtr);
    }

    public unsafe IPipelineResourceSignature UnpackResourceSignature(ResourceSignatureUnpackInfo unpackInfo)
    {
        var resourceSignPtr = IntPtr.Zero;
        var unpackInfoData = ResourceSignatureUnpackInfo.GetInternalStruct(unpackInfo);
        var unpackInfoPtr = &unpackInfoData;

        Interop.dearchiver_unpack_resource_signature(Handle, new IntPtr(unpackInfoPtr), new IntPtr(&resourceSignPtr));
        if (resourceSignPtr == IntPtr.Zero)
            throw new NullReferenceException("Failed to unpack resource signature");
        return NativeObjectRegistry.GetOrCreate<IPipelineResourceSignature>(() => throw new NotImplementedException(),
            resourceSignPtr);
    }

    public unsafe IRenderPass UnpackRenderPass(RenderPassUnpackInfo unpackInfo)
    {
        var renderPassPtr = IntPtr.Zero;
        var unpackInfoData = RenderPassUnpackInfo.GetInternalStruct(unpackInfo);
        var unpackInfoPtr = &unpackInfoData;

        Interop.dearchiver_unpack_resource_signature(Handle, new IntPtr(unpackInfoPtr), new IntPtr(&renderPassPtr));
        if (renderPassPtr == IntPtr.Zero)
            throw new NullReferenceException("Failed to unpack resource signature");
        return NativeObjectRegistry.GetOrCreate<IRenderPass>(() => throw new NotImplementedException(),
            renderPassPtr);
    }

    public unsafe bool Store(out IDataBlob? dataBlob)
    {
        var dataBlobPtr = IntPtr.Zero;
        var result = Interop.dearchiver_store(Handle, new IntPtr(&dataBlobPtr));

        dataBlob = CreateDataBlob(dataBlobPtr);
        return result;

        IDataBlob? CreateDataBlob(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
                return null;
            return NativeObjectRegistry.GetOrCreate(() => new DataBlob(handle), handle);
        }
    }

    public void Reset()
    {
        Interop.dearchiver_reset(Handle);
    }
}
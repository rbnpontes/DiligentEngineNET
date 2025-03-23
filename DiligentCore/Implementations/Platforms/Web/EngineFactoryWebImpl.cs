using System.Runtime.CompilerServices;
using Diligent.Utils;

namespace Diligent.Platforms.Default.Web;

internal class EngineFactoryWebImpl(IntPtr handle) : IEngineFactoryImpl
{
    public unsafe APIInfo GetApiInfo()
    {
        var apiInfo = new APIInfo.__Internal();
        var ptr = EngineFactory.WebInterop.engine_factory_get_apiinfo(handle);
        WebInteropUtils.CopyToManaged(ptr, &apiInfo, (uint)Unsafe.SizeOf<APIInfo>());
        return APIInfo.FromInternalStruct(apiInfo);
    }

    public void SetBreakOnError(bool value)
    {
        EngineFactory.WebInterop.engine_factory_set_break_on_error(handle, value);
    }

    public unsafe IShaderSourceInputStreamFactory GetShaderSourceInputStreamFactory(string searchDirectories)
    {
        using var strAlloc = new StringAllocator();
        var factoryPtr = WebScratchBuffer.Require(sizeof(nint));
        var resultPtr = IntPtr.Zero;
        
        EngineFactory.Interop.engine_factory_create_default_shader_source_stream_factory(handle, 
            strAlloc.Acquire(searchDirectories), 
            factoryPtr);
        WebInteropUtils.CopyToManaged(factoryPtr, &resultPtr, (uint)sizeof(nint));
        return DiligentObjectsFactory.CreateShaderSourceInputStreamFactory(resultPtr);
    }

    public IDataBlob CreateDataBlob(ulong initialSize) => CreateDataBlob(initialSize, IntPtr.Zero);

    public unsafe IDataBlob CreateDataBlob(ulong initialSize, IntPtr data)
    {
        using var mem = new WebMemory((uint)initialSize);
        var resultPtr = IntPtr.Zero;
        WebInteropUtils.CopyToDiligent(mem, data.ToPointer(), (uint)initialSize);
        
        var dataBlobPtr = WebScratchBuffer.Require(sizeof(nint));
        EngineFactory.WebInterop.engine_factory_create_data_blob(handle, (long)initialSize, mem.Handle, dataBlobPtr);
        WebInteropUtils.CopyToManaged(dataBlobPtr, &resultPtr, (uint)sizeof(nint));
        
        return DiligentObjectsFactory.CreateDataBlob(resultPtr);
    }

    public unsafe IDataBlob CreateDataBlob<T>(ref T data) where T : struct
    {
        var initialSize = (ulong)Unsafe.SizeOf<T>();
        return CreateDataBlob(initialSize, new IntPtr(Unsafe.AsPointer(ref data)));
    }

    public unsafe IDataBlob CreateDataBlob<T>(ReadOnlySpan<T> data) where T : unmanaged
    {
        var initialSize = (ulong)(Unsafe.SizeOf<T>() * data.Length);
        fixed (T* dataPtr = data)
            return CreateDataBlob(initialSize, new IntPtr(dataPtr));
    }

    public unsafe IDearchiver CreateDearchiver(DearchiverCreateInfo createInfo)
    {
        var requiredSize = Unsafe.SizeOf<DearchiverCreateInfo.__Internal>() + sizeof(nint);
        var buffer = WebScratchBuffer.Require(requiredSize);
        
        var createInfoInternal = DearchiverCreateInfo.GetInternalStruct(createInfo);
        WebInteropUtils.CopyToDiligent(buffer, &createInfoInternal, (uint)Unsafe.SizeOf<DearchiverCreateInfo.__Internal>());
        
        EngineFactory.WebInterop.engine_factory_create_dearchiver(
            handle,
            buffer,
            buffer + Unsafe.SizeOf<DearchiverCreateInfo.__Internal>());
        
        var dearchiverPtr = IntPtr.Zero;
        WebInteropUtils.CopyToManaged(
            buffer + Unsafe.SizeOf<DearchiverCreateInfo.__Internal>(),
            &dearchiverPtr,
            (uint)sizeof(nint)
        );
        
        return DiligentObjectsFactory.CreateDearchiver(dearchiverPtr);
    }

    public unsafe GraphicsAdapterInfo[] EnumerateAdapters(Version minVersion)
    {
        var structSize = Unsafe.SizeOf<Version.__Internal>();
        var buffer = WebScratchBuffer.Require(
            structSize + sizeof(nint)
        );
        var version = Version.GetInternalStruct(minVersion);
        uint numAdapters = 0;
        
        WebInteropUtils.CopyToDiligent(buffer, &version, (uint)structSize);
        EngineFactory.WebInterop.engine_factory_enumerate_adapters(
            handle,
            buffer,
            buffer + structSize,
            IntPtr.Zero);
        WebInteropUtils.CopyToManaged(buffer + structSize, &numAdapters, (uint)sizeof(nint));

        if (numAdapters == 0)
            return [];
        
        var adapters = new GraphicsAdapterInfo.__Internal[numAdapters];
        using var adaptersMem = new WebMemory((uint)Unsafe.SizeOf<GraphicsAdapterInfo.__Internal>() * numAdapters);
        
        EngineFactory.WebInterop.engine_factory_enumerate_adapters(
            handle,
            buffer,
            buffer + structSize,
            adaptersMem.Handle);
        fixed (GraphicsAdapterInfo.__Internal* adaptersPtr = adapters)
            WebInteropUtils.CopyToManaged(adaptersMem.Handle, adaptersPtr, adaptersMem.Size);
        
        return adapters.Select(GraphicsAdapterInfo.FromInternalStruct).ToArray();
    }
}
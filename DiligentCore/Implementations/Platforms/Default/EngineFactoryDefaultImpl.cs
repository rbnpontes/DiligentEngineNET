using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Diligent.Utils;

namespace Diligent.Platforms.Default;

internal unsafe class EngineFactoryDefaultImpl(IntPtr handle) : IEngineFactoryImpl
{
    public APIInfo GetApiInfo()
    {
        var data = (APIInfo.__Internal*)EngineFactory.Interop.engine_factory_get_apiinfo(handle).ToPointer();
        return APIInfo.FromInternalStruct(*data);
    }

    public void SetBreakOnError(bool value)
    {
        EngineFactory.Interop.engine_factory_set_break_on_error(handle, value);
    }

    public IShaderSourceInputStreamFactory GetShaderSourceInputStreamFactory(string searchDirectories)
    {
        using var strAlloc = new StringAllocator();
        var factoryPtr = IntPtr.Zero;
        EngineFactory.Interop.engine_factory_create_default_shader_source_stream_factory(handle, 
            strAlloc.Acquire(searchDirectories), 
            new IntPtr(&factoryPtr));
        return DiligentObjectsFactory.CreateShaderSourceInputStreamFactory(factoryPtr);
    }

    public IDataBlob CreateDataBlob(ulong initialSize)
    {
        return CreateDataBlob(initialSize, IntPtr.Zero);
    }

    public IDataBlob CreateDataBlob(ulong initialSize, IntPtr data)
    {
        var dataBlobPtr = IntPtr.Zero;
        EngineFactory.Interop.engine_factory_create_data_blob(handle, initialSize, data, new IntPtr(&dataBlobPtr));
        return DiligentObjectsFactory.CreateDataBlob(dataBlobPtr);
    }

    public IDataBlob CreateDataBlob<T>(ref T data) where T : struct
    {
        var initialSize = (ulong)Marshal.SizeOf<T>();
        return CreateDataBlob(initialSize, new IntPtr(Unsafe.AsPointer(ref data)));
    }

    public IDataBlob CreateDataBlob<T>(ReadOnlySpan<T> data) where T : unmanaged
    {
        var initialSize = (ulong)(Marshal.SizeOf<T>() * data.Length);
        fixed (T* dataPtr = data)
            return CreateDataBlob(initialSize, new IntPtr(dataPtr));
    }

    public IDearchiver CreateDearchiver(DearchiverCreateInfo createInfo)
    {
        var createInfoInternal = DearchiverCreateInfo.GetInternalStruct(createInfo);
        var dearchiverPtr = IntPtr.Zero;
        EngineFactory.Interop.engine_factory_create_dearchiver(handle, 
            new IntPtr(&createInfoInternal), 
            new IntPtr(&dearchiverPtr));
        return DiligentObjectsFactory.CreateDearchiver(dearchiverPtr);
    }

    public GraphicsAdapterInfo[] EnumerateAdapters(Version minVersion)
    {
        var versionStruct = Version.GetInternalStruct(minVersion);
        uint numAdapters = 0;
        
        EngineFactory.Interop.engine_factory_enumerate_adapters(handle, 
            new IntPtr(&versionStruct), 
            new IntPtr(&numAdapters),
            IntPtr.Zero);

        if (numAdapters == 0)
            return [];

        var adaptersInternals = new GraphicsAdapterInfo.__Internal[numAdapters];
        fixed (GraphicsAdapterInfo.__Internal* adaptersPtr = adaptersInternals)
        {
            EngineFactory.Interop.engine_factory_enumerate_adapters(
                handle,
                new IntPtr(&versionStruct),
                new IntPtr(&numAdapters),
                new IntPtr(adaptersPtr));
        }

        return adaptersInternals.Select(GraphicsAdapterInfo.FromInternalStruct).ToArray();
    }
}
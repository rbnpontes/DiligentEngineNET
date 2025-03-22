using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Diligent.Utils;

namespace Diligent;

internal abstract partial class EngineFactory : IEngineFactory
{
    private static MessageCallbackHandler _callbackHandler;

    
    public unsafe APIInfo APIInfo
    {
        get
        {
            var data = (APIInfo.__Internal*)Interop.engine_factory_get_apiinfo(Handle).ToPointer();
            return APIInfo.FromInternalStruct(*data);
        }
    }

    internal EngineFactory(IntPtr handle) : base(handle)
    {
        if(PlatformUtils.IsWasm)
            _callbackHandler = new WebMessageCallbackHandler(handle);
        else
            _callbackHandler = new DefaultMessageCallbackHandler(handle);
    }
    
    protected override void Release()
    {
    }

    protected override int AddRef()
    {
        return 0;
    }

    public unsafe GraphicsAdapterInfo[] EnumerateAdapters(Version minVersion)
    {
        var versionStruct = Version.GetInternalStruct(minVersion);
        uint numAdapters = 0;
        
        Interop.engine_factory_enumerate_adapters(Handle, 
            new IntPtr(&versionStruct), 
            new IntPtr(&numAdapters),
            IntPtr.Zero);

        if (numAdapters == 0)
            return [];

        var adaptersInternals = new GraphicsAdapterInfo.__Internal[numAdapters];
        fixed (GraphicsAdapterInfo.__Internal* adaptersPtr = adaptersInternals)
        {
            Interop.engine_factory_enumerate_adapters(
                Handle,
                new IntPtr(&versionStruct),
                new IntPtr(&numAdapters),
                new IntPtr(adaptersPtr));
        }

        return adaptersInternals.Select(GraphicsAdapterInfo.FromInternalStruct).ToArray();
    }

    public void SetMessageCallback(DebugMessageCallbackDelegate? messageCallback)
    {
        _callbackHandler.SetCallback(messageCallback);
    }

    public void SetBreakOnError(bool breakOnError)
    {
        Interop.engine_factory_set_break_on_error(Handle, breakOnError);
    }

    public unsafe IShaderSourceInputStreamFactory CreateDefaultShaderSourceStreamFactory(string searchDirectories)
    {
        using var strAlloc = new StringAllocator();
        var factoryPtr = IntPtr.Zero;
        Interop.engine_factory_create_default_shader_source_stream_factory(Handle, 
            strAlloc.Acquire(searchDirectories), 
            new IntPtr(&factoryPtr));
        return DiligentObjectsFactory.CreateShaderSourceInputStreamFactory(factoryPtr);
    }

    public IDataBlob CreateDataBlob(ulong initialSize)
    {
        return CreateDataBlob(initialSize, IntPtr.Zero);
    }

    public unsafe IDataBlob CreateDataBlob(ulong initialSize, IntPtr data)
    {
        var dataBlobPtr = IntPtr.Zero;
        Interop.engine_factory_create_data_blob(Handle, initialSize, data, new IntPtr(&dataBlobPtr));
        return DiligentObjectsFactory.CreateDataBlob(dataBlobPtr);
    }

    public unsafe IDataBlob CreateDataBlob<T>(ref T data) where T : struct
    {
        var initialSize = (ulong)Marshal.SizeOf<T>();
        return CreateDataBlob(initialSize, new IntPtr(Unsafe.AsPointer(ref data)));
    }

    public unsafe IDataBlob CreateDataBlob<T>(ReadOnlySpan<T> data) where T : unmanaged
    {
        var initialSize = (ulong)(Marshal.SizeOf<T>() * data.Length);
        fixed (T* dataPtr = data)
            return CreateDataBlob(initialSize, new IntPtr(dataPtr));
    }

    public unsafe IDearchiver CreateDearchiver(DearchiverCreateInfo createInfo)
    {
        var createInfoInternal = DearchiverCreateInfo.GetInternalStruct(createInfo);
        var dearchiverPtr = IntPtr.Zero;
        Interop.engine_factory_create_dearchiver(Handle, 
            new IntPtr(&createInfoInternal), 
            new IntPtr(&dearchiverPtr));
        return DiligentObjectsFactory.CreateDearchiver(dearchiverPtr);
    }

    protected int GetNumDeferredContexts(EngineCreateInfo createInfo)
    {
        return (int)(int.Max((int)createInfo.NumImmediateContexts, 1) + createInfo.NumDeferredContexts);
    }
}
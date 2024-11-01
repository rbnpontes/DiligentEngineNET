using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Diligent;

public abstract partial class EngineFactory : IEngineFactory
{
    // prevent destruction of delegate when unmanaged code call
    private static DebugMessageCallbackDelegate GlobalMessageCallback = DummyMessageCallbackCall;
    public unsafe APIInfo APIInfo
    {
        get
        {
            var data = (APIInfo.__Internal*)Interop.engine_factory_get_apiinfo(Handle).ToPointer();
            return APIInfo.FromInternalStruct(*data);
        }
    }

    public EngineFactory(): base(){}
    internal EngineFactory(IntPtr handle) : base(handle)
    {
    }

    ~EngineFactory()
    {
        GlobalMessageCallback = DummyMessageCallbackCall;
    }
    public unsafe GraphicsAdapterInfo[] EnumerateAdapters(Version minVersion)
    {
        var versionStruct = Version.GetInternalStruct(minVersion);
        uint numAdapters = 0;

        uint* numAdaptersPtr = &numAdapters;
        Version.__Internal* minVersionPtr = &versionStruct;

        Interop.engine_factory_enumerate_adapters(Handle, new IntPtr(minVersionPtr), new IntPtr(numAdaptersPtr),
            IntPtr.Zero);

        if (numAdapters == 0)
            return [];

        var adaptersInternals = new GraphicsAdapterInfo.__Internal[numAdapters];
        fixed (GraphicsAdapterInfo.__Internal* adaptersPtr = adaptersInternals)
        {
            Interop.engine_factory_enumerate_adapters(
                Handle,
                new IntPtr(minVersionPtr),
                new IntPtr(numAdaptersPtr),
                new IntPtr(adaptersPtr));
        }

        return adaptersInternals.Select(GraphicsAdapterInfo.FromInternalStruct).ToArray();
    }

    public void SetMessageCallback(DebugMessageCallbackDelegate messageCallback)
    {
        GlobalMessageCallback = messageCallback;
        var functionPtr = Marshal.GetFunctionPointerForDelegate(GlobalMessageCallback);
        EngineFactory.Interop.engine_factory_set_message_callback(Handle, functionPtr);
    }

    public void SetBreakOnError(bool breakOnError)
    {
        Interop.engine_factory_set_break_on_error(Handle, breakOnError);
    }

    public unsafe IShaderSourceInputStreamFactory CreateDefaultShaderSourceStreamFactory(string searchDirectories)
    {
        var searchDirPtr = Marshal.StringToHGlobalAnsi(searchDirectories);
        var factoryPtr = IntPtr.Zero;
        Interop.engine_factory_create_default_shader_source_stream_factory(Handle, searchDirPtr, new IntPtr(&factoryPtr));
        Marshal.FreeHGlobal(searchDirPtr);

        if (factoryPtr == IntPtr.Zero)
            throw new NullReferenceException($"Failed to create {nameof(IShaderSourceInputStreamFactory)}.");
        return new ShaderSourceInputStreamFactory(factoryPtr);
    }

    public IDataBlob CreateDataBlob(ulong initialSize)
    {
        return CreateDataBlob(initialSize, IntPtr.Zero);
    }

    public unsafe IDataBlob CreateDataBlob(ulong initialSize, IntPtr data)
    {
        var dataBlobPtr = IntPtr.Zero;
        Interop.engine_factory_create_data_blob(Handle, initialSize, data, new IntPtr(&dataBlobPtr));

        if (dataBlobPtr == IntPtr.Zero)
            throw new NullReferenceException("Failed to create IDataBlob. Diligent Core returns null");

        return new DataBlob(dataBlobPtr);
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
        {
            return CreateDataBlob(initialSize, new IntPtr(dataPtr));
        }
    }

    private static void DummyMessageCallbackCall(DebugMessageSeverity severity, string message, string function, string file, int line) {}
}
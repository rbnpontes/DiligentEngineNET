using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Diligent.Utils;

namespace Diligent;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal delegate void NativeDebugMessageCallbackDelegate(DebugMessageSeverity severity, IntPtr message, IntPtr function,
    IntPtr file, int line);
internal abstract partial class EngineFactory : IEngineFactory
{
    private DebugMessageCallbackDelegate? _callback;
    private NativeDebugMessageCallbackDelegate _nativeDelegate;
    private IntPtr _nativeDelegatePtr;

    
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
        _nativeDelegate = NativeMessageCallbackCall;
        _nativeDelegatePtr = Marshal.GetFunctionPointerForDelegate(_nativeDelegate);
        Interop.engine_factory_set_message_callback(handle, _nativeDelegatePtr);
    }

    protected override void Release()
    {
        if (_nativeDelegatePtr == IntPtr.Zero)
            return;
        
        _nativeDelegatePtr = IntPtr.Zero;
        _callback = null;
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
        _callback = messageCallback;
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

    private void NativeMessageCallbackCall(DebugMessageSeverity severity, IntPtr message, IntPtr function,
        IntPtr file, int line)
    {
        if (_callback is null)
            return;

        var messageStr = Marshal.PtrToStringAnsi(message) ?? string.Empty;
        var funcStr = Marshal.PtrToStringAnsi(function) ?? string.Empty;
        var fileStr = Marshal.PtrToStringAnsi(file) ?? string.Empty;

        _callback(severity, messageStr, funcStr, fileStr, line);
    }

    protected int GetNumDeferredContexts(EngineCreateInfo createInfo)
    {
        return (int)(int.Max((int)createInfo.NumImmediateContexts, 1) + createInfo.NumDeferredContexts);
    }
}
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Diligent.Platforms.Default;
using Diligent.Platforms.Default.Web;
using Diligent.Utils;

namespace Diligent;

internal abstract partial class EngineFactory : IEngineFactory
{
    private static MessageCallbackHandler _callbackHandler;
    private IEngineFactoryImpl _impl;

    
    public APIInfo APIInfo => _impl.GetApiInfo();

    internal EngineFactory(IntPtr handle) : base(handle)
    {
        if (PlatformUtils.IsWasm)
        {
            _callbackHandler = new WebMessageCallbackHandler(handle);
            _impl = new EngineFactoryWebImpl(handle);
            return;
        }
        
        _callbackHandler = new DefaultMessageCallbackHandler(handle);
        _impl = new EngineFactoryDefaultImpl(handle);
    }
    
    protected override void Release()
    {
    }

    protected override int AddRef()
    {
        return 0;
    }

    public unsafe GraphicsAdapterInfo[] EnumerateAdapters(Version minVersion) => _impl.EnumerateAdapters(minVersion);

    public void SetMessageCallback(DebugMessageCallbackDelegate? messageCallback)
    {
        _callbackHandler.SetCallback(messageCallback);
    }

    public void SetBreakOnError(bool breakOnError) => _impl.SetBreakOnError(breakOnError);

    public unsafe IShaderSourceInputStreamFactory CreateDefaultShaderSourceStreamFactory(string searchDirectories) => _impl.GetShaderSourceInputStreamFactory(searchDirectories);

    public IDataBlob CreateDataBlob(ulong initialSize) => _impl.CreateDataBlob(initialSize);

    public unsafe IDataBlob CreateDataBlob(ulong initialSize, IntPtr data) => _impl.CreateDataBlob(initialSize, data);

    public unsafe IDataBlob CreateDataBlob<T>(ref T data) where T : struct => _impl.CreateDataBlob(ref data);

    public unsafe IDataBlob CreateDataBlob<T>(ReadOnlySpan<T> data) where T : unmanaged => _impl.CreateDataBlob(data);

    public unsafe IDearchiver CreateDearchiver(DearchiverCreateInfo createInfo) => _impl.CreateDearchiver(createInfo);
    protected static int GetNumDeferredContexts(EngineCreateInfo createInfo)
    {
        return (int)(int.Max((int)createInfo.NumImmediateContexts, 1) + createInfo.NumDeferredContexts);
    }
}
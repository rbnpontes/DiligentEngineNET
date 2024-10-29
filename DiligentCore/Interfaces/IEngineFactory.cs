namespace Diligent;

public delegate void DebugMessageCallbackDelegate(DebugMessageSeverity severity, string message, string function,
    string file, int line);
public interface IEngineFactory : IDiligentObject
{
    APIInfo APIInfo { get; }
    GraphicsAdapterInfo[] EnumerateAdapters(Version minVersion);
    void SetMessageCallback(DebugMessageCallbackDelegate messageCallback);
    void SetBreakOnError(bool breakOnError);
    IDataBlob CreateDataBlob(ulong initialSize);
    IDataBlob CreateDataBlob(ulong initialSize, IntPtr data);
    IDataBlob CreateDataBlob<T>(T data) where T : struct;
    IDataBlob CreateDataBlob<T>(ReadOnlySpan<T> data) where T : unmanaged;
}
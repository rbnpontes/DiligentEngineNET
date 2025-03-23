namespace Diligent.Platforms.Default;

internal interface IEngineFactoryImpl
{
    public APIInfo GetApiInfo();
    public void SetBreakOnError(bool value);
    public IShaderSourceInputStreamFactory GetShaderSourceInputStreamFactory(string searchDirectories);
    public IDataBlob CreateDataBlob(ulong initialSize);
    public IDataBlob CreateDataBlob(ulong initialSize, IntPtr data);
    public IDataBlob CreateDataBlob<T>(ref T data) where T : struct;
    public IDataBlob CreateDataBlob<T>(ReadOnlySpan<T> data) where T : unmanaged;
    public IDearchiver CreateDearchiver(DearchiverCreateInfo createInfo);
    public GraphicsAdapterInfo[] EnumerateAdapters(Version minVersion);
}
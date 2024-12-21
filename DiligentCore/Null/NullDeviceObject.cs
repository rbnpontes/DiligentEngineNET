namespace Diligent.Null;

public class NullDeviceObject : IDeviceObject
{
    public IntPtr Handle { get; private set; }

    public bool IsDisposed { get; private set; }
    public IReferenceCounters ReferenceCounters => new NullReferenceCounters();

    public DeviceObjectAttribs Desc => new DeviceObjectAttribs()
    {
        Name = nameof(NullDeviceObject)
    };

    public int UniqueId => GetHashCode();
    public IDiligentObject? UserData { get; set; }

    protected NullDeviceObject(IntPtr handle)
    {
        Handle = handle;
        NativeObjectRegistry.AddToRegister(handle, this);
    }

    ~NullDeviceObject()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        GC.SuppressFinalize(this);
        NativeObjectRegistry.RemoveObject(Handle);
    }
}
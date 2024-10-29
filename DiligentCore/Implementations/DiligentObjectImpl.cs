namespace Diligent;

public partial class DiligentObject : IDiligentObject
{
    private IntPtr _handle;
    public IntPtr Handle => _handle;
    
    public bool IsDisposed
    {
        get => _handle == IntPtr.Zero;
    }

    public ReferenceCounters ReferenceCounters => new(
        Interop.object_get_reference_counters(Handle),
        this
    );

    public DiligentObject()
    {
        throw new NotSupportedException("Constructor without parameters isn't supported.");
    }
    
    public DiligentObject(IntPtr handle)
    {
        _handle = handle;
        AddRef();
    }

    ~DiligentObject()
    {
        Release();
    }


    private void AssertDispose()
    {
        if (_handle == IntPtr.Zero)
            throw new ObjectDisposedException("Object already disposed");
    }
    
    private ReferenceCounters GetReferenceCounters()
    {
        AssertDispose();
        return new ReferenceCounters(Interop.object_get_reference_counters(Handle), this);
    }
    
    public void Dispose()
    {
        AssertDispose();
        Release();
        GC.SuppressFinalize(this);
    }

    public virtual int AddRef()
    {
        AssertDispose();
        return DiligentObject.Interop.object_add_ref(Handle);
    }

    public virtual void Release()
    {
        if (_handle == IntPtr.Zero)
            return;
        DiligentObject.Interop.object_release(Handle);
        _handle = IntPtr.Zero;
    }
}
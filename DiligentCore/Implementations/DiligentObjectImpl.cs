namespace Diligent;

internal partial class DiligentObject : NativeObject, IDiligentObject
{
    public bool IsDisposed { get; private set; }

    public IReferenceCounters ReferenceCounters => GetReferenceCounters();

    public DiligentObject() : base(IntPtr.Zero)
    {
        throw new NotSupportedException("Constructor without parameters isn't supported.");
    }
    
    public DiligentObject(IntPtr handle) : base(handle)
    {
        NativeObjectRegistry.AddToRegister(handle, this);
    }

    ~DiligentObject()
    {
        Dispose();
    }

    protected override IntPtr GetCurrentHandle()
    {
        AssertDispose();
        return base.GetCurrentHandle();
    }

    private void AssertDispose()
    {
        if (IsDisposed)
            throw new ObjectDisposedException("Object already disposed");
    }
    
    private ReferenceCounters GetReferenceCounters()
    {
        var pointer = Interop.object_get_reference_counters(Handle);
        if (NativeObjectRegistry.TryGetObject(pointer, out var output))
            return output as ReferenceCounters ?? throw new InvalidOperationException();
        return new ReferenceCounters(pointer, this);
    }
    
    public void Dispose()
    {
        if(IsDisposed)
            return;
        
        GC.SuppressFinalize(this);
        NativeObjectRegistry.RemoveObject(Handle);
        Release();
        SetCurrentHandle(IntPtr.Zero);
        IsDisposed = true;
    }

    protected virtual int AddRef()
    {
        AssertDispose();
        return Interop.object_add_ref(Handle);
    }

    
    protected virtual void Release()
    {
        if (IsDisposed)
            return;
        
        Interop.object_release(Handle);
    }
}
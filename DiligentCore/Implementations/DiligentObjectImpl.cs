using Diligent.Utils;

namespace Diligent;

internal partial class DiligentObject : NativeObject, IDiligentObject
{
    public bool IsDisposed { get; private set; }

    public IReferenceCounters ReferenceCounters => DiligentObjectsFactory.CreateReferenceCounters(
        Interop.object_get_reference_counters(Handle),
        this);

    protected DiligentObject() : base(IntPtr.Zero)
    {
        throw new NotSupportedException("Constructor without parameters isn't supported.");
    }

    protected DiligentObject(IntPtr handle) : base(handle)
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
    
    public void Dispose()
    {
        if(IsDisposed)
            return;
        IsDisposed = true;
        GC.SuppressFinalize(this);
        NativeObjectRegistry.RemoveObject(InternalHandle);
        Release();
        SetCurrentHandle(IntPtr.Zero);
    }

    internal void DisposeInternal()
    {
        if (IsDisposed)
            return;
        
        IsDisposed = true;
        GC.SuppressFinalize(this);
        SetCurrentHandle(IntPtr.Zero);
    }

    protected virtual int AddRef()
    {
        return Interop.object_add_ref(Handle);
    }
    
    protected virtual void Release()
    {
        Interop.object_release(InternalHandle);
    }
}

public class UnknownObject : NativeObject, IDiligentObject
{
     public bool IsDisposed { get; private set; }
     public IReferenceCounters ReferenceCounters => DiligentObjectsFactory.CreateReferenceCounters(
         DiligentObject.Interop.object_get_reference_counters(Handle),
         this);

     internal UnknownObject(IntPtr handle) : base(handle)
     {
     }

     ~UnknownObject()
     {
         Dispose();    
     }
     
     public void Dispose()
     {
         if (IsDisposed)
             return;
         
         GC.SuppressFinalize(this);
         SetCurrentHandle(IntPtr.Zero);
         IsDisposed = true;
     }

}
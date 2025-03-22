using Diligent.Utils;

namespace Diligent;

internal partial class DiligentObject : NativeObject, IDiligentObject
{
    public bool IsDisposed { get; private set; }

    public IReferenceCounters ReferenceCounters => GetOrCreateReferenceCounters();
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
        return PlatformUtils.IsWasm ? AddRefWeb() : AddRefDefault();

        int AddRefDefault() => Interop.object_add_ref(Handle);
        int AddRefWeb() => WebInterop.object_add_ref(Handle);
    }
    
    protected virtual void Release()
    {
        if(PlatformUtils.IsWasm)
            ReleaseWeb();
        else
            ReleaseDefault();
        return;
        
        void ReleaseDefault() => Interop.object_release(InternalHandle);
        void ReleaseWeb() => WebInterop.object_release(InternalHandle);
    }

    private IReferenceCounters GetOrCreateReferenceCounters()
    {
        var ptr = PlatformUtils.IsWasm 
            ? WebInterop.object_get_reference_counters(Handle) 
            : Interop.object_get_reference_counters(Handle);
        return DiligentObjectsFactory.CreateReferenceCounters(ptr, this);
    }
}

public class UnknownObject : NativeObject, IDiligentObject
{
     public bool IsDisposed { get; private set; }
     public IReferenceCounters ReferenceCounters => GetOrCreateReferenceCounters();

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

     private IReferenceCounters GetOrCreateReferenceCounters()
     {
         var ptr = PlatformUtils.IsWasm 
             ? DiligentObject.WebInterop.object_get_reference_counters(Handle) 
             : DiligentObject.Interop.object_get_reference_counters(Handle);
         return DiligentObjectsFactory.CreateReferenceCounters(ptr, this);
     }
}
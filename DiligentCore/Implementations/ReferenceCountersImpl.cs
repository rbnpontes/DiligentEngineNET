namespace Diligent;

public partial class ReferenceCounters : NativeObject
{
    private WeakReference<INativeObject> _owner;

    public int NumStrongRefs
    {
        get
        {
            AssertAlive();
            return Interop.reference_counters_get_num_strong_refs(Handle);
        }
    }

    public int NumWeakRefs
    {
        get
        {
            AssertAlive();
            return Interop.reference_counters_get_num_weak_refs(Handle);
        }   
    }
    
    internal ReferenceCounters(IntPtr handle, INativeObject owner) : base(handle)
    {
        _owner = new WeakReference<INativeObject>(owner);
    }

    private void AssertAlive()
    {
        if (!_owner.TryGetTarget(out var _))
            throw new NativeObjectDestroyedException(typeof(ReferenceCounters));
    }

    public int AddStrongRef()
    {
        AssertAlive();
        return Interop.reference_counters_add_strong_ref(Handle);
    }

    public int ReleaseStrongRef()
    {
        AssertAlive();
        return Interop.reference_counters_release_strong_ref(Handle);
    }

    public int AddWeakRef()
    {
        AssertAlive();
        return Interop.reference_counters_add_weak_ref(Handle);
    }

    public int ReleaseWeakRef()
    {
        AssertAlive();
        return Interop.reference_counters_release_weak_ref(Handle);
    }
}
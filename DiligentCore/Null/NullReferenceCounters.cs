namespace Diligent.Null;

public class NullReferenceCounters : IReferenceCounters
{
    private bool _disposed;
    public IntPtr Handle => IntPtr.Zero;
    public int NumStrongRefs { get; private set; } = 1;
    public int NumWeakRefs { get; private set; }

    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        --NumStrongRefs;
    }

    public int AddStrongRef()
    {
        return ++NumStrongRefs;
    }

    public int ReleaseStrongRef()
    {
        if (NumStrongRefs == 1)
        {
            Dispose();
            return NumStrongRefs;
        }

        return --NumStrongRefs;
    }

    public int AddWeakRef()
    {
        return ++NumWeakRefs;
    }

    public int ReleaseWeakRef()
    {
        if (NumWeakRefs == 0)
            return NumWeakRefs;
        return --NumWeakRefs;
    }
}
namespace Diligent;

public interface IReferenceCounters : INativeObject, IDisposable
{
    int NumStrongRefs { get; }
    int NumWeakRefs { get; }
    int AddStrongRef();
    int ReleaseStrongRef();
    int AddWeakRef();
    int ReleaseWeakRef();
}
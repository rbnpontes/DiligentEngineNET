namespace Diligent;

public interface IDiligentObject : INativeObject, IDisposable
{
    bool IsDisposed { get; }
    ReferenceCounters ReferenceCounters { get; }
}
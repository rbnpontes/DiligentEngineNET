namespace Diligent;

public interface ICommandQueue : IDiligentObject
{
    ulong NextFenceValue { get; }
    ulong CompletedFenceValue { get; }
    ulong WaitForIdle();
}
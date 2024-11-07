namespace Diligent;

public interface ISwapChain : IDiligentObject
{
    void Present(uint syncInterval = 1);
}
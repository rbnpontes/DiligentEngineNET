namespace Diligent;

internal partial class SwapChain : ISwapChain
{
    internal SwapChain(IntPtr handle) : base(handle)
    {
    }

    public void Present(uint syncInterval = 1)
    {
        Interop.swap_chain_present(Handle, syncInterval);
    }
}
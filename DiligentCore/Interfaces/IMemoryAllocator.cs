namespace Diligent;

public interface IMemoryAllocator
{
    IntPtr Allocate(ulong size, string dbgDescription, string dbgFileName, int dbgLineNumber);
    void Free(IntPtr pointer);
    IntPtr AllocateAligned(ulong size, ulong alignment, string dbgDescription, string dbgFileName, int dbgLineNumber);
    void FreeAligned(IntPtr pointer);
}
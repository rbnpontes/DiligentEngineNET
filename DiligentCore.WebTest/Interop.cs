using System.Runtime.InteropServices.JavaScript;
using Diligent;

public static partial class Interop
{
    [JSImport("globalThis.setRenderLoop")]
    public static partial void SetRenderLoop([JSMarshalAs<JSType.Function>] Action callback);
    [JSImport("_malloc", Constants.WebLibName)]
    public static partial IntPtr MemoryAlloc(int size);
    [JSImport("_free", Constants.WebLibName)]
    public static partial void MemoryFree(IntPtr memory);
    
    [JSImport("memory_copy_net_2_diligent", Constants.WebLibName)]
    public static partial void MemoryCopyNet2Diligent(IntPtr memory, [JSMarshalAs<JSType.MemoryView>] Span<byte> buffer, int size);

    [JSImport("stringToNewUTF8", Constants.WebLibName)]
    public static partial IntPtr AllocString(string str);
}
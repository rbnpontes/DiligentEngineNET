using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using Diligent.Utils;

namespace Diligent;

internal static partial class WebInteropUtils
{
    [JSImport("_malloc", Constants.WebLibName)]
    public static partial IntPtr MemoryAlloc(int size);
    [JSImport("_free", Constants.WebLibName)]
    public static partial void MemoryFree(IntPtr ptr);
    [JSImport("_memset", Constants.WebLibName)]
    public static partial void MemorySet(IntPtr memory, int value, int size);
    [JSImport("stringToNewUTF8", Constants.WebLibName)]
    public static partial IntPtr AllocString(string value);
    
    [JSImport("UTF8ToString", Constants.WebLibName)]
    public static partial string GetString(IntPtr ptr);
    
    [JSImport("memory_copy_net_2_diligent", Constants.WebLibName)]
    private static partial void MemoryCopyNet2Diligent(IntPtr memory, [JSMarshalAs<JSType.MemoryView>] Span<byte> buffer, int size);
    [JSImport("memory_copy_diligent_2_net", Constants.WebLibName)]
    private static partial void MemoryCopyDiligent2Net(IntPtr memory, [JSMarshalAs<JSType.MemoryView>] Span<byte> buffer, int size);

    [JSImport("delegate_register", Constants.WebLibName)]
    public static partial IntPtr DelegateRegister([JSMarshalAs<JSType.Function<JSType.Number>>] Action<int> call, string signature);
    
    [JSImport("delegate_unregister", Constants.WebLibName)]
    public static partial void DelegateUnregister(IntPtr ptr);
    
    public static void CopyToDiligent(IntPtr diligentPtr, Span<byte> buffer)
    {
		MemoryCopyNet2Diligent(diligentPtr, buffer, buffer.Length);    
    }
    
    public static void CopyToDiligent(IntPtr diligentPtr, Span<byte> data, uint size)
    {
		MemoryCopyNet2Diligent(diligentPtr, data, (int)size);		
    }
    
    public static unsafe void CopyToDiligent(IntPtr diligentPtr, void* data, uint size)
    {
	    CopyToDiligent(diligentPtr, new Span<byte>(data, (int)size));
    }

    public static void CopyToDiligent(IntPtr diligentPtr, byte[] data)
    {
	    CopyToDiligent(diligentPtr, data.AsSpan());
    }
    
    public static unsafe void CopyToDiligent(WebMemory memory, void* data, uint size)
    {
	    CopyToDiligent(memory.Handle, data, size);
    }

    public static void CopyToDiligent(WebMemory memory, byte[] data)
    {
	    CopyToDiligent(memory.Handle, data);
    }
    
    public static void CopyToManaged(IntPtr diligentPtr, Span<byte> data)
    {
	    CopyToManaged(diligentPtr, data, (uint)data.Length);
    }

    public static void CopyToManaged(IntPtr diligentPtr, Span<byte> data, uint size)
    {
	    MemoryCopyDiligent2Net(diligentPtr, data, (int)size);
    }

    public static unsafe void CopyToManaged(IntPtr diligentPtr, void* data, uint size)
    {
	    CopyToManaged(diligentPtr, new Span<byte>(data, (int)size));
    }

    public static unsafe void CopyToManaged(IntPtr diligentPtr, byte[] data)
    {
	    CopyToManaged(diligentPtr, data.AsSpan());
    }
    
    public static void CopyToManaged(WebMemory memory, Span<byte> data)
    {
	    CopyToManaged(memory.Handle, data);
    }

    public static void CopyToManaged(WebMemory memory, Span<byte> data, uint size)
    {
	    CopyToManaged(memory.Handle, data, size);
    }

    public static unsafe void CopyToManaged(WebMemory memory, void* data, uint size)
    {
	    CopyToManaged(memory.Handle, data, size);
    }

    public static unsafe void CopyToManaged(WebMemory memory, byte[] data)
    {
	    CopyToManaged(memory.Handle, data);
    }
}
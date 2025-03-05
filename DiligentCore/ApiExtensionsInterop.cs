using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Diligent;

internal delegate void DiligentReleaseCalback(IntPtr obj, IntPtr refCount);
internal static unsafe partial class ApiExtensionsInterop
{
    [LibraryImport((Constants.LibName))]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int diligent_core_api_ext_get_api_version();

    [LibraryImport((Constants.LibName))]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial void diligent_core_api_set_release_callback(IntPtr callback);
    
    public static void SetReleaseCallback(delegate* unmanaged[Cdecl]<void*, void*, void> callbackPtr)
    {
        diligent_core_api_set_release_callback(new IntPtr(callbackPtr));
    }
}
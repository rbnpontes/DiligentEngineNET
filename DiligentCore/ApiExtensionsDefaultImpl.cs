using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Diligent;

internal partial class ApiExtensionsDefaultImpl : IApiExtensions
{
    private delegate void DiligentReleaseCalback(IntPtr obj, IntPtr refCount);

    private static Action<nint, nint> sReleaseCallback = (_, _)=> {};
    private static nint sReleaseCallbackPtr = nint.Zero;
    
    [LibraryImport((Constants.LibName))]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial int diligent_core_api_ext_get_api_version();

    [LibraryImport((Constants.LibName))]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial void diligent_core_api_set_release_callback(IntPtr callback);
    
    public int GetApiVersion() => diligent_core_api_ext_get_api_version();

    public void SetReleaseCallback(Action<IntPtr, IntPtr> releaseCallback)
    {
        sReleaseCallback = releaseCallback;
        if (sReleaseCallbackPtr != nint.Zero)
            return;
        sReleaseCallbackPtr = Marshal.GetFunctionPointerForDelegate(OnReleaseCallback);
    }

    private static void OnReleaseCallback(IntPtr obj, IntPtr refCount)
    {
        sReleaseCallback(obj, refCount);
    }
}
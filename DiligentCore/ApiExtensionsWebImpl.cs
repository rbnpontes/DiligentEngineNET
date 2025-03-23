using System.Runtime.InteropServices.JavaScript;
using Diligent.Utils;

namespace Diligent;

internal partial class ApiExtensionsWebImpl : IApiExtensions
{
    private static Action<nint, nint> sReleaseCallback = (_, _)=> {};
    private static nint sReleaseCallbackPtr = nint.Zero;
    
    [JSImport("_diligent_core_api_ext_get_api_version", Constants.WebLibName)]
    private static partial int diligent_core_api_ext_get_api_version();
    [JSImport("_diligent_core_api_set_release_callback")]
    private static partial void diligent_core_api_set_release_callback(IntPtr callbackPtr);

    ~ApiExtensionsWebImpl()
    {
        WebInteropUtils.DelegateUnregister(sReleaseCallbackPtr);
    }
    
    public int GetApiVersion() => diligent_core_api_ext_get_api_version();

    public void SetReleaseCallback(Action<IntPtr, IntPtr> releaseCallback)
    {
        sReleaseCallback = releaseCallback;
        if (sReleaseCallbackPtr != nint.Zero)
            return;
        
        sReleaseCallbackPtr = WebInteropUtils.DelegateRegister(OnReleaseCallback, "vii");
    }

    private static void OnReleaseCallback(int objIdx)
    {
        var objPtr = JsObjectUtils.ArrayGetPtr(objIdx, 0);
        var refCountPtr = JsObjectUtils.ArrayGetPtr(objIdx, 1);
        sReleaseCallback.Invoke(objPtr, refCountPtr);
    }
}
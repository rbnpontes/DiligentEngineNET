using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Diligent.Utils;

namespace Diligent;

internal delegate void DiligentReleaseCalback(IntPtr obj, IntPtr refCount);
internal static partial class ApiExtensionsInterop
{
    private static IApiExtensions? _instance;

    public static IApiExtensions GetInstance()
    {
        if(_instance is not null)
            return _instance;
        
        if(PlatformUtils.IsWasm)
            _instance = new ApiExtensionsWebImpl();
        else
            _instance = new ApiExtensionsDefaultImpl();
        
        return _instance;
    }
}
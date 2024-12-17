using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Diligent;

internal static partial class ApiExtensionsInterop
{
    [LibraryImport((Constants.LibName))]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    public static partial int diligent_core_api_ext_get_api_version();
}
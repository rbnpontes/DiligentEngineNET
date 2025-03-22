using System.Runtime.InteropServices.JavaScript;
using Diligent;

namespace Diligent.Utils;

internal static partial class JsObjectUtils
{
    [JSImport("object_list__array_get", Constants.WebLibName)]
    public static partial int ArrayGetInt(int objIdx, int arrayIdx);
    [JSImport("object_list__array_get", Constants.WebLibName)]
    public static partial IntPtr ArrayGetPtr(int objIdx, int arrayIdx);
}
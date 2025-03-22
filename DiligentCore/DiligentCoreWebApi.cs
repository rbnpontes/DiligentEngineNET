using System.Runtime.InteropServices.JavaScript;

namespace Diligent;

public static partial class DiligentCore
{
    internal static partial class WebInterop
    {
        [JSImport("_diligent_core_get_opengl_factory", Constants.WebLibName)]
        public static partial IntPtr diligent_core_get_opengl_factory();
        
        [JSImport("_diligent_core_get_webgpu_factory", Constants.WebLibName)]
        public static partial IntPtr diligent_core_get_webgpu_factory();
    }
}
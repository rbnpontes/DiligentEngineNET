using System.Runtime.InteropServices.JavaScript;

public static partial class Interop
{
    [JSImport("globalThis.setRenderLoop")]
    public static partial void SetRenderLoop([JSMarshalAs<JSType.Function>] Action callback);
}
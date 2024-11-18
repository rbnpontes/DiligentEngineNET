namespace CodeGenerator;

public static class ExclusionList
{
    public static string[] Methods =
    [
        "CreateSwapChainD3D11",
        "CreateSwapChainD3D12",
        "CreateSwapChainVk",
        "CreateDeviceAndSwapChainGL",
        "AttachToActiveGLContext"
    ];

    public static string[] Classes =
    [
        "EngineGLCreateInfo",
        "Win32NativeWindow",
        "LinuxNativeWindow",
        "EmscriptenNativeWindow",
        "TVOSNativeWindow",
        "IOSNativeWindow",
        "MacOSNativeWindow",
        "AndroidNativeWindow"
    ];

    public static string[] PropertiesToSkip =
    [
        "ResourceSignaturesCount",
        "ppResourceSignatures",
        "pInternalData",
        "GeneralShaderCount",
        "TriangleHitShaderCount",
        "ProceduralHitShaderCount",
    ];
}
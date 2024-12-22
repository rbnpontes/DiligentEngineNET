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
        "AndroidNativeWindow",
    ];

    public static string[] PropertiesToSkip =
    [
        "ResourceSignaturesCount",
        "ppResourceSignatures",
        "pInternalData",
        "GeneralShaderCount",
        "TriangleHitShaderCount",
        "ProceduralHitShaderCount",
        "MultiDrawIndexedAttribs::pDrawItems",
        "InputLayoutDesc::LayoutElements",
        "SubpassDesc::pResolveAttachments",
        "SubpassDesc::pDepthStencilAttachment",
        "SubpassDesc::pShadingRateAttachment",
        "MultiDrawAttribs::pDrawItems",
        "ShaderCreateInfo::Macros",
    ];

    public static Dictionary<string, string[]> PlatformSpecificClasses = new()
    {
        { "IEngineFactoryD3D11", ["PLATFORM_WIN32", "PLATFORM_UNIVERSAL_PLATFORM"] },
        { "IEngineFactoryD3D12", ["PLATFORM_WIN32", "PLATFORM_UNIVERSAL_PLATFORM"] },
    };
}
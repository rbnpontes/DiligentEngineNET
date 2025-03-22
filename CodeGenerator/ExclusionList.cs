namespace CodeGenerator;

public static class ExclusionList
{
    public static string[] Methods =
    [
        "CreateSwapChainD3D11",
        "CreateSwapChainD3D12",
        "CreateSwapChainVk",
        "CreateDeviceAndSwapChainGL",
        "CreateSwapChainWebGPU",
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
        { "IEngineFactoryVk", [
            "PLATFORM_WIN32", 
            "PLATFORM_UNIVERSAL_PLATFORM", 
            "PLATFORM_ANDROID",
            "PLATFORM_IOS",
            "PLATFORM_MACOS",
            "PLATFORM_TVOS"
        ] },
        { "IEngineFactoryWebGPU", ["PLATFORM_WEB"] }
    };

    public static HashSet<string> IgnoreFromList = new()
    {
        "IEngineFactoryD3D11",
        "IEngineFactoryD3D12",
        "IEngineFactoryVk",
    };
}
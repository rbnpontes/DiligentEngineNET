public static class Remapper
{
    public static readonly Dictionary<string, string> EnumItemRemap = new()
    {
        { "CreateShaderSourceInputStreamFlagSilent", "Silent" },
        { "TextureAddressMirror", "Mirror" },
        { "TextureAddressMirrorOnce", "MirrorOnce" },
        { "ShaderCompileFlagSkipReflection", "SkipReflection" }
    };

    public static readonly HashSet<string> Enums2Flags = new()
    {
        "SHADER_TYPE"
    };
}
public static class Remapper
{
    public static readonly Dictionary<string, string> EnumItemRemap = new()
    {
        { "CreateShaderSourceInputStreamFlagSilent", "Silent" },
        { "TextureAddressMirror", "Mirror" },
        { "TextureAddressMirrorOnce", "MirrorOnce" },
    };

    public static readonly HashSet<string> Enums2Flags = new()
    {
        "SHADER_TYPE"
    };
}
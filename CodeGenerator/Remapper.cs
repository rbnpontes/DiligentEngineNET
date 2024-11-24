public static class Remapper
{
    public static readonly Dictionary<string, string> EnumItemRemap = new()
    {
        { "CreateShaderSourceInputStreamFlagSilent", "Silent" },
        { "BlendFactorBlendFactor", "BlendFactor" }
    };

    public static readonly HashSet<string> Enums2Flags = new()
    {
        "SHADER_TYPE"
    };
}
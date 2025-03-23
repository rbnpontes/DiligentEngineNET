namespace Diligent;

public static partial class Constants
{
    public const string LibName = "DiligentCore";
    public const string WebLibName = $"lib{LibName}.js";
    public static int DiligentApiVersion => ApiExtensionsInterop.GetInstance().GetApiVersion();
}
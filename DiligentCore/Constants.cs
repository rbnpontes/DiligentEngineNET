namespace Diligent;

public static partial class Constants
{
    public const string LibName = "DiligentCore";
    public static int DiligentApiVersion => ApiExtensionsInterop.diligent_core_api_ext_get_api_version();
}
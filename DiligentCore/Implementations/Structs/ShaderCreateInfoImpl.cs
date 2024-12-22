namespace Diligent;

public partial class ShaderCreateInfo
{
    /// <summary>
    /// ByteCode Data, if you have unmanaged data pointer
    /// you can use ByteCode property.
    /// </summary>
    public byte[] ByteCodeData { get; set; } = [];
    public ShaderMacro[] Macros { get; set; } = [];
    public ShaderCreateInfo()
    {
        EntryPoint = "main";
    }
}
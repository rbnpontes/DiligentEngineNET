namespace Diligent;

public sealed partial class ShaderMacro
{
    public ShaderMacro(){}

    public ShaderMacro(string name, string definition)
    {
        Name = name;
        Definition = definition;
    }
}
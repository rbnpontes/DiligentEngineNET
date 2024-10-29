using System.Runtime.InteropServices;

namespace Diligent;

public unsafe partial class GraphicsAdapterInfo
{
    public string Description
    {
        get
        {
            fixed(sbyte* desc = _data.Description)
                return new string(desc);
        }
        set
        {
            for (var i = 0; i < value.Length; ++i)
                _data.Description[i] = (sbyte)value[i];
        }
    }
}
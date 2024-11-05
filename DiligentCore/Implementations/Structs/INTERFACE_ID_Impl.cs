namespace Diligent;

public partial class INTERFACE_ID
{
    public INTERFACE_ID()
    {
    }
    
    // similar initialization signature of CPP code
    public unsafe INTERFACE_ID(uint data1, ushort data2, ushort data3, byte[] data4)
    {
        _data.Data1 = data1;
        _data.Data2 = data2;
        _data.Data3 = data3;
        for (var i = 0; i < Math.Min(data4.Length, 8); ++i)
            _data.Data4[i] = data4[i];
    }
}
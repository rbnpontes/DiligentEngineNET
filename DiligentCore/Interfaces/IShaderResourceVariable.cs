namespace Diligent;

public interface IShaderResourceVariable : IDiligentObject
{
    ShaderResourceVariableType Type { get; }
    ShaderResourceDesc ResourceDesc { get; }
    uint Index { get; }
    
    void Set(IDeviceObject? obj, SetShaderResourceFlags flags = SetShaderResourceFlags.None);

    void SetArray(IDeviceObject[] objects, uint firstElement,
        SetShaderResourceFlags flags = SetShaderResourceFlags.None);

    void SetBufferRange(IDeviceObject? obj, ulong offset, ulong size, uint arrayIndex = 0,
        SetShaderResourceFlags flags = SetShaderResourceFlags.None);

    void SetBufferOffset(uint offset, uint arrayIndex = 0);

    IDeviceObject? Get(uint arrayIndex = 0);
}
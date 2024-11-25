namespace Diligent;

internal partial class ShaderResourceVariable(IntPtr handle) : DiligentObject(handle), IShaderResourceVariable
{
    public ShaderResourceVariableType Type { get; }
    public ShaderResourceDesc ResourceDesc { get; }
    public uint Index { get; }

    protected override void Release()
    {
    }

    public void Set(IDeviceObject? obj, SetShaderResourceFlags flags = SetShaderResourceFlags.None)
    {
        throw new NotImplementedException();
    }

    public void SetArray(IDeviceObject[] objects, uint firstElement, SetShaderResourceFlags flags = SetShaderResourceFlags.None)
    {
        throw new NotImplementedException();
    }

    public void SetBufferRange(IDeviceObject? obj, ulong offset, ulong size, uint arrayIndex = 0,
        SetShaderResourceFlags flags = SetShaderResourceFlags.None)
    {
        throw new NotImplementedException();
    }

    public void SetBufferOffset(uint offset, uint arrayIndex = 0)
    {
        throw new NotImplementedException();
    }

    public IDeviceObject? Get(uint arrayIndex = 0)
    {
        throw new NotImplementedException();
    }
}
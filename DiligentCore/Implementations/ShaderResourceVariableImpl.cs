using Diligent.Utils;

namespace Diligent;

internal unsafe partial class ShaderResourceVariable(IntPtr handle) : DiligentObject(handle), IShaderResourceVariable
{
    public ShaderResourceVariableType Type => Interop.shader_resource_variable_get_type(Handle);

    public ShaderResourceDesc ResourceDesc
    {
        get
        {
            var shaderResourceDesc = new ShaderResourceDesc.__Internal();
            Interop.shader_resource_variable_get_resource_desc(Handle, new IntPtr(&shaderResourceDesc));
            return DiligentDescFactory.GetShaderResourceDesc(new IntPtr(&shaderResourceDesc));
        }   
    }

    public uint Index => Interop.shader_resource_variable_get_index(Handle);

    protected override void Release()
    {
    }

    public void Set(IDeviceObject? obj, SetShaderResourceFlags flags = SetShaderResourceFlags.None)
    {
        Interop.shader_resource_variable_set(Handle, obj?.Handle ?? IntPtr.Zero, flags);
    }

    public void SetArray(IDeviceObject[] objects, uint firstElement, SetShaderResourceFlags flags = SetShaderResourceFlags.None)
    {
        var objectsPtr = objects.Select(x => x.Handle).ToArray();
        fixed(void* ptr = objectsPtr)
            Interop.shader_resource_variable_set_array(Handle, new IntPtr(ptr), firstElement, (uint)objects.Length, flags);
    }

    public void SetBufferRange(IDeviceObject? obj, ulong offset, ulong size, uint arrayIndex = 0,
        SetShaderResourceFlags flags = SetShaderResourceFlags.None)
    {
        Interop.shader_resource_variable_set_buffer_range(Handle,
            obj?.Handle ?? IntPtr.Zero,
            offset,
            size,
            arrayIndex,
            flags);
    }

    public void SetBufferOffset(uint offset, uint arrayIndex = 0)
    {
        Interop.shader_resource_variable_set_buffer_offset(Handle, offset, arrayIndex);
    }

    public IDeviceObject? Get(uint arrayIndex = 0)
    {
        var ptr = Interop.shader_resource_variable_get(Handle, arrayIndex);
        return ptr == IntPtr.Zero ? null : NativeObjectRegistry.GetOrCreate(()=> new UnknownDeviceObject(ptr), ptr);
    }
}
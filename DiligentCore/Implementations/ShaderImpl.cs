using Diligent.Utils;

namespace Diligent;

unsafe partial class Shader : IShader
{
    public uint ResourceCount => Interop.shader_get_resource_count(Handle);
    public Shader(IntPtr handle) : base(handle){}
    public ShaderResourceDesc GetResourceDesc(uint index)
    {
        var descPtr = IntPtr.Zero;
        Interop.shader_get_resource_desc(Handle, index, new IntPtr(&descPtr));
        return DiligentDescFactory.GetShaderResourceDesc(descPtr);
    }

    public ShaderCodeBufferDesc GetConstantBufferDesc(uint index)
    {
        var descPtr = Interop.shader_get_constant_buffer_desc(Handle, index);
        return DiligentDescFactory.GetShaderCodeBufferDesc(descPtr);
    }

    public byte[] GetBytecode()
    {
        var byteCode = IntPtr.Zero;
        var byteCodeLength = 0;
        Interop.shader_get_bytecode(Handle, new IntPtr(&byteCode), new IntPtr(&byteCodeLength));
        return new Span<byte>(byteCode.ToPointer(), byteCodeLength).ToArray();
    }

    public ShaderStatus GetStatus(bool waitForCompletion = false)
    {
        return Interop.shader_get_status(Handle, waitForCompletion);
    }
}
using System.Runtime.InteropServices;

namespace Diligent;

internal partial class ShaderSourceInputStreamFactory : IShaderSourceInputStreamFactory
{
    public ShaderSourceInputStreamFactory() : base() {}
    internal ShaderSourceInputStreamFactory(IntPtr ptr) : base(ptr) {}
    
    public unsafe IFileStream CreateInputStream(string name)
    {
        var namePtr = Marshal.StringToHGlobalAnsi(name);
        var fileStreamPtr = IntPtr.Zero;
        Interop.shader_source_input_stream_factory_create_input_stream(Handle, namePtr, new IntPtr(&fileStreamPtr));
        return CreateInputStreamFromPointer(fileStreamPtr);
    }

    public unsafe IFileStream CreateInputStream(string name, CreateShaderSourceInputStreamFlags flags)
    {
        var namePtr = Marshal.StringToHGlobalAnsi(name);
        var fileStreamPtr = IntPtr.Zero;
        Interop.shader_source_input_stream_factory_create_input_stream_2(Handle, namePtr, flags, new IntPtr(&fileStreamPtr));
        return CreateInputStreamFromPointer(fileStreamPtr);
    }

    private IFileStream CreateInputStreamFromPointer(IntPtr ptr)
    {
        if (ptr == IntPtr.Zero)
            throw new NullReferenceException($"Failed to create {nameof(IFileStream)}");
        return new FileStream(ptr);
    }
}
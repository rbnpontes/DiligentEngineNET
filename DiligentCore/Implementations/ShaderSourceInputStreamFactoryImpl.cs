using System.Runtime.InteropServices;
using Diligent.Utils;

namespace Diligent;

internal partial class ShaderSourceInputStreamFactory : IShaderSourceInputStreamFactory
{
    internal ShaderSourceInputStreamFactory(IntPtr ptr) : base(ptr) {}
    
    public unsafe IFileStream CreateInputStream(string name)
    {
        using var strAlloc = new StringAllocator();
        var fileStreamPtr = IntPtr.Zero;
        Interop.shader_source_input_stream_factory_create_input_stream(Handle, 
            strAlloc.Acquire(name), 
            new IntPtr(&fileStreamPtr));
        return DiligentObjectsFactory.CreateInputStream(fileStreamPtr);
    }

    public unsafe IFileStream CreateInputStream(string name, CreateShaderSourceInputStreamFlags flags)
    {
        using var strAlloc = new StringAllocator();
        var fileStreamPtr = IntPtr.Zero;
        Interop.shader_source_input_stream_factory_create_input_stream_2(Handle, 
            strAlloc.Acquire(name), 
            flags, 
            new IntPtr(&fileStreamPtr));
        return DiligentObjectsFactory.CreateInputStream(fileStreamPtr);
    }
}
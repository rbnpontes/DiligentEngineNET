using System.Reflection;
using System.Runtime.InteropServices;
using Diligent.Utils;

namespace Diligent;

public static partial class DiligentCore
{
    internal static partial class Interop
    {
        [LibraryImport(Constants.LibName)]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr diligent_core_get_d3d11_factory();

        [LibraryImport(Constants.LibName)]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr diligent_core_get_d3d12_factory();

        [LibraryImport(Constants.LibName)]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr diligent_core_get_vk_factory();
        
        [LibraryImport(Constants.LibName)]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial IntPtr diligent_core_get_opengl_factory();
    }

    private static DiligentReleaseCalback sReleaseCalback;
    static DiligentCore()
    {
        try
        {
            NativeLibrary.SetDllImportResolver(typeof(DiligentCore).Assembly, DiligentLibraryResolver.Resolver);
        }
        catch
        {
            // If a previous assembly register an importer resolver
            // an exception will be raised when this assembly tries
            // to register a self resolver.We must skip to prevent
            // issues at Assembly loading.
        }
        
        // listen to diligent object destruction
        sReleaseCalback = HandleDiligentRelease;
        var delegatePtr = Marshal.GetFunctionPointerForDelegate(sReleaseCalback);
        ApiExtensionsInterop.diligent_core_api_set_release_callback(delegatePtr);
    }

    private static void HandleDiligentRelease(IntPtr objPtr, IntPtr refCountPtr)
    {
        var obj = NativeObjectRegistry.TryGetObject(objPtr);
        var refCount = NativeObjectRegistry.TryGetObject(refCountPtr);
        
        if(obj is DiligentObject targetObj)
            targetObj.DisposeInternal();
        if(refCount is ReferenceCounters targetRefCount)
           targetRefCount.DisposeInternal();
        
        NativeObjectRegistry.RemoveObject(objPtr);
        NativeObjectRegistry.RemoveObject(refCountPtr);
    }
    
    public static IEngineFactoryD3D11? GetEngineFactoryD3D11()
    {
        var ptr = Interop.diligent_core_get_d3d11_factory();
        if (ptr == IntPtr.Zero)
            return null;
        if (NativeObjectRegistry.TryGetObject(ptr, out var output))
            return output as IEngineFactoryD3D11;
        return new EngineFactoryD3D11(ptr);
    }

    public static IEngineFactoryD3D12? GetEngineFactoryD3D12()
    {
        var ptr = Interop.diligent_core_get_d3d12_factory();
        if (ptr == IntPtr.Zero)
            return null;
        if (NativeObjectRegistry.TryGetObject(ptr, out var output))
            return output as IEngineFactoryD3D12;
        return new EngineFactoryD3D12(ptr);
    }

    public static IEngineFactoryVk? GetEngineFactoryVk()
    {
        var ptr = Interop.diligent_core_get_vk_factory();
        if (ptr == IntPtr.Zero)
            return null;
        if (NativeObjectRegistry.TryGetObject(ptr, out var output))
            return output as IEngineFactoryVk;
        return new EngineFactoryVk(ptr);
    }

    public static IEngineFactoryOpenGL? GetEngineFactoryOpenGL()
    {
        var ptr = Interop.diligent_core_get_opengl_factory();
        if (ptr == IntPtr.Zero)
            return null;
        if (NativeObjectRegistry.TryGetObject(ptr, out var output))
            return output as IEngineFactoryOpenGL;
        return new EngineFactoryOpenGL(ptr);
    }
}
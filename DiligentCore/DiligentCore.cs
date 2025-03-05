using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
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
        if (!OperatingSystem.IsWindows())
            SetupInternalLibrary();
    }

    private static void SetupInternalLibrary()
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

        SetupReleaseFunction();
    }

    private static unsafe void SetupReleaseFunction()
    {
        // listen to diligent object destruction
        ApiExtensionsInterop.SetReleaseCallback(&HandleDiligentRelease);
    }

    public static void SetupLibrary()
    {
        if (!OperatingSystem.IsBrowser())
            return;
        SetupInternalLibrary();
    }


    [UnmanagedCallersOnly(CallConvs =
    [
        typeof(CallConvCdecl)
    ])]
    private static unsafe void HandleDiligentRelease(void* arg0, void* arg1)
    {
        var objPtr = (IntPtr)arg0;
        var refCountPtr = (IntPtr)arg1;
        var obj = NativeObjectRegistry.TryGetObject(objPtr);
        var refCount = NativeObjectRegistry.TryGetObject(refCountPtr);

        if (obj is DiligentObject targetObj)
            targetObj.DisposeInternal();
        if (refCount is ReferenceCounters targetRefCount)
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
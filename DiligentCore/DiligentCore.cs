using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using Diligent.Utils;

namespace Diligent;

public static partial class DiligentCore
{
    private static IFactoryBuilder _builder = PlatformUtils.IsWasm 
        ? new WebFactoryBuilder() 
        : new DefaultFactoryBuilder();
    
    private static DiligentReleaseCalback sReleaseCalback;

    static DiligentCore()
    {
        if (!PlatformUtils.IsWasm)
            SetupInternalLibrary();
        SetupReleaseFunction();
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
    }

    private static void SetupReleaseFunction()
    {
        // listen to diligent object destruction
        ApiExtensionsInterop.GetInstance().SetReleaseCallback(HandleDiligentRelease);
    }


    private static unsafe void HandleDiligentRelease(nint objPtr, nint refCountPtr)
    {
        var obj = NativeObjectRegistry.TryGetObject(objPtr);
        var refCount = NativeObjectRegistry.TryGetObject(refCountPtr);

        if (obj is DiligentObject targetObj)
            targetObj.DisposeInternal();
        if (refCount is ReferenceCounters targetRefCount)
            targetRefCount.DisposeInternal();

        NativeObjectRegistry.RemoveObject(objPtr);
        NativeObjectRegistry.RemoveObject(refCountPtr);
    }

    public static IEngineFactoryD3D11? GetEngineFactoryD3D11() => _builder.GetEngineFactoryD3D11();

    public static IEngineFactoryD3D12? GetEngineFactoryD3D12() => _builder.GetEngineFactoryD3D12();

    public static IEngineFactoryVk? GetEngineFactoryVk() => _builder.GetEngineFactoryVk();

    public static IEngineFactoryOpenGL? GetEngineFactoryOpenGL() => _builder.GetEngineFactoryOpenGL();

    public static IEngineFactoryWebGPU? GetEngineFactoryWebGPU() => _builder.GetEngineFactoryWebGPU();
}
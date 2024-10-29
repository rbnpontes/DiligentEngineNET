namespace Diligent;

public partial class EngineFactoryD3D11 : IEngineFactoryD3D11
{
    internal EngineFactoryD3D11(IntPtr handle) : base(handle){}

    public (IDeviceContext[], IRenderDevice) CreateDeviceAndContexts(EngineCreateInfo createInfo)
    {
        throw new NotImplementedException();
    }

    public ISwapChain CreateSwapChain(IRenderDevice device, IDeviceContext immediateContext, SwapChainDesc swapChainDesc,
        FullScreenModeDesc fullScreenDesc, NativeWindow window)
    {
        throw new NotImplementedException();
    }

    public unsafe DisplayModeAttribs[] EnumerateDisplayModes(Version minFeatureLevel, uint adapterId, uint outputId, TextureFormat format)
    {
        uint numDisplayModes = 0;
        var minFeatureLvlStruct = Version.GetInternalStruct(minFeatureLevel);
        
        Version.__Internal* versionPtr = &minFeatureLvlStruct;
        uint* numDisplayModesPtr = &numDisplayModes;

        Interop.engine_factory_d3d11_enumerate_display_modes(
            Handle,
            new IntPtr(versionPtr),
            adapterId,
            outputId,
            format,
            new IntPtr(numDisplayModesPtr),
            IntPtr.Zero);

        if (numDisplayModes == 0)
            return [];

        var displayModes = new DisplayModeAttribs.__Internal[numDisplayModes];
        fixed (DisplayModeAttribs.__Internal* displayModesPtr = displayModes)
            Interop.engine_factory_d3d11_enumerate_display_modes(
                Handle,
                new IntPtr(versionPtr),
                adapterId,
                outputId,
                format,
                new IntPtr(numDisplayModesPtr),
                new IntPtr(displayModesPtr));

        return displayModes.Select(DisplayModeAttribs.FromInternalStruct).ToArray();
    }
}
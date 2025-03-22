namespace Diligent;

internal interface IFactoryBuilder
{
    public IEngineFactoryD3D11? GetEngineFactoryD3D11();
    public IEngineFactoryD3D12? GetEngineFactoryD3D12();
    public IEngineFactoryVk? GetEngineFactoryVk();
    public IEngineFactoryOpenGL? GetEngineFactoryOpenGL();
    public IEngineFactoryWebGPU? GetEngineFactoryWebGPU();
}
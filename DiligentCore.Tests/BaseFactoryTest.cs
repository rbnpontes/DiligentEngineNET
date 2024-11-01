namespace Diligent.Tests;

public class BaseFactoryTest
{
    protected IEngineFactory GetFactory()
    {
        IEngineFactory? result;
        if (OperatingSystem.IsWindows())
            result = DiligentCore.GetEngineFactoryD3D11();
        else
            result = DiligentCore.GetEngineFactoryVk();
        return result ?? throw new NullReferenceException();
    }
}
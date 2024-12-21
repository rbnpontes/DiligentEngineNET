using Diligent.Tests.Utils;

namespace Diligent.Tests;

public class BaseFactoryTest
{
    protected IEngineFactory GetFactory()
    {
        var result = DiligentCore.GetEngineFactoryVk() ?? throw new NullReferenceException();
        result.SetMessageCallback(DebugOutput.MessageCallback);
        return result;
    }
}
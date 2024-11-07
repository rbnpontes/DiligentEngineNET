using Diligent.Tests.Utils;

namespace Diligent.Tests;

[TestFixture]
public class EngineFactoryVkTest
{
    private IEngineFactoryVk GetFactory()
    {
        var factory = DiligentCore.GetEngineFactoryVk() ?? throw new NullReferenceException();
        factory.SetMessageCallback(DebugOutput.MessageCallback);
        return factory;
    }

    [Test]
    public void MustCreateDeviceAndContext()
    {
        using var factory = GetFactory();
        var createInfo = new EngineVkCreateInfo()
        {
            EnableValidation = true,
        };
    }
}
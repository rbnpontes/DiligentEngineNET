using System.Runtime.CompilerServices;

namespace Diligent.Tests;

public class DiligentCoreTest
{
    private readonly Func<IEngineFactory?>[] _getFactoryCalls =
    [
        DiligentCore.GetEngineFactoryD3D11,
        DiligentCore.GetEngineFactoryD3D12,
        DiligentCore.GetEngineFactoryVk,
        DiligentCore.GetEngineFactoryOpenGL
    ];
    
    [Test]
    public void MustGetFactoryD3D11()
    {
        if (!OperatingSystem.IsWindows())
        {
            Assert.Pass();
            return;
        }
        
        Assert.That(DiligentCore.GetEngineFactoryD3D11(), Is.Not.Null);
    }

    [Test]
    public void MustGetFactoryD3D12()
    {
        if(!OperatingSystem.IsWindows())
            return;
        
        Assert.That(DiligentCore.GetEngineFactoryD3D12(), Is.Not.Null);
    }
    
    [Test]
    public void MustGetFactoryVulkan()
    {
        Assert.That(DiligentCore.GetEngineFactoryVk(), Is.Not.Null);
    }
    
    [Test]
    public void MustGetFactoryOpenGL()
    {
        Assert.That(DiligentCore.GetEngineFactoryOpenGL(), Is.Not.Null);
    }

    [Test]
    public void MustReturnSameReference()
    {
        foreach (var getFactoryCall in _getFactoryCalls)
        {
            var firstRef = getFactoryCall();
            var secondRef = getFactoryCall();
            
            Assert.That(firstRef, Is.Not.Null);
            Assert.That(firstRef, Is.EqualTo(secondRef));
            Assert.That(firstRef.GetHashCode(), Is.EqualTo(secondRef.GetHashCode()));
        }
    }
    
    [Test]
    public void MustNotReturnSameReferenceIfPrevInstanceHasBeenReleased()
    {
        int firstHashCode; 
        int secondHashCode;
        foreach (var getFactoryCall in _getFactoryCalls)
        {
            var instance = getFactoryCall();
            firstHashCode = instance?.GetHashCode() ?? 0;
            instance?.Dispose();
            instance = getFactoryCall();
            secondHashCode = instance?.GetHashCode() ?? 0;
            instance?.Dispose();
            
            Assert.That(firstHashCode, Is.Not.EqualTo(secondHashCode));
        }
    }
}
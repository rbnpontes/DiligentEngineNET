namespace Diligent.Tests.Structs;

[TestFixture]
public class EngineD3D12CreateInfoTest
{
     [Test]
     public void MustSetCorrectDefault()
     {
          var createInfo = new EngineD3D12CreateInfo();
          Assert.That(createInfo.D3D12DllName, Is.EqualTo("d3d12.dll"));
     }
}
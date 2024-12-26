namespace Diligent.Tests;

[TestFixture]
[NonParallelizable]
public class DeviceContextTest : BaseRenderTest
{
    [Test]
    public void MustTransitionResources()
    {
        using var buffer = Device.CreateBuffer(new BufferDesc()
        {
            Mode = BufferMode.Raw,
            Name = "Test",
            Usage = Usage.Dynamic,
            Size = 1024,
            BindFlags = BindFlags.ShaderResource | BindFlags.UniformBuffer,
            CPUAccessFlags = CpuAccessFlags.Write
        });

        Context.TransitionResourceStates([
            new StateTransitionDesc()
            {
                OldState = ResourceState.Unknown,
                NewState = ResourceState.ShaderResource,
                Resource = buffer
            }
        ]);
    }
}
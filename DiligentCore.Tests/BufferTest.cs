namespace Diligent.Tests;

[TestFixture]
public class BufferTest : BaseRenderTest
{
    [Test]
    public void MustCreateBuffer()
    {
        var bufferDesc = new BufferDesc()
        {
            Name = "Test Buffer",
            Size = 1024,
            BindFlags = BindFlags.UniformBuffer,
            Usage = Usage.Dynamic,
            CPUAccessFlags = CpuAccessFlags.Write
        };
        using var buffer = Device.CreateBuffer(bufferDesc);
        Assert.Pass();
    }

    [Test]
    public void MustThrowErrorIfBufferCreationFails()
    {
        var bufferDesc = new BufferDesc()
        {
            Name = "Test Buffer",
            Size = 1024,
            BindFlags = BindFlags.UniformBuffer,
            Usage = Usage.Dynamic,
        };

        Assert.Throws<NullReferenceException>(() =>
        {
            using var buffer = Device.CreateBuffer(bufferDesc);
        });
    }

    [Test]
    public void MustGetDesc()
    {
        var bufferDesc = new BufferDesc()
        {
            Name = "Test Buffer",
            Size = 1024,
            BindFlags = BindFlags.UniformBuffer,
            Usage = Usage.Dynamic,
            CPUAccessFlags = CpuAccessFlags.Write
        };
        using var buffer = Device.CreateBuffer(bufferDesc);
        Assert.That(buffer.Desc.Name, Is.EqualTo(bufferDesc.Name));
    }
}
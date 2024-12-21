namespace Diligent.Tests;

[TestFixture]
[NonParallelizable]
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
    public void MustCreateWithInitialData()
    {
        var data = new byte[1024];
        Array.Fill<byte>(data, 0xFF);
        
        var bufferDesc = new BufferDesc()
        {
            Name = "Test Buffer",
            Size = 1024,
            BindFlags = BindFlags.VertexBuffer,
            Usage = Usage.Immutable,
        };
        using var buffer = Device.CreateBuffer(bufferDesc, data.AsSpan());
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

    [Test]
    public void MustGetDefaultView()
    {
        var initialData = new byte[1024];
        var bufferDesc = new BufferDesc()
        {
            Name = "Test Buffer",
            Size = 1024,
            BindFlags = BindFlags.UnorderedAccess,
            Mode = BufferMode.Raw,
            Usage = Usage.Default,
        };
        using var buffer = Device.CreateBuffer(bufferDesc, initialData.AsSpan());
        using var view = buffer.GetDefaultView(BufferViewType.UnorderedAccess);
        
        Assert.That(view, Is.Not.Null);
    }
}
namespace Diligent.Tests;

[TestFixture]
[NonParallelizable]
public class ResourceMappingTest : BaseRenderTest
{
    private IBuffer CreateBuffer()
    {
        return Device.CreateBuffer(new BufferDesc()
        {
            Name = "Test Buffer",
            BindFlags = BindFlags.UniformBuffer,
            Size = 1024,
            Usage = Usage.Dynamic,
            CPUAccessFlags = CpuAccessFlags.Write
        });
    }
    
    [Test]
    public void MustCreate()
    {
        using var buffer = CreateBuffer();
        var createInfo = new ResourceMappingCreateInfo()
        {
            Entries =
            [
                new ResourceMappingEntry()
                {
                    Name = "test",
                    Object = buffer,
                }
            ]
        };
        using var resMapping = Device.CreateResourceMapping(createInfo);
        Assert.That(resMapping, Is.Not.Null);
    }

    [Test]
    public void MustGetSize()
    {
        using var buffer = CreateBuffer();
        var createInfo = new ResourceMappingCreateInfo()
        {
            Entries =
            [
                new ResourceMappingEntry()
                {
                    Name = "test",
                    Object = buffer,
                }
            ]
        };
        using var resMapping = Device.CreateResourceMapping(createInfo);
        Assert.That(resMapping.Size, Is.GreaterThan(0));
    }

    [Test]
    public void MustGetResource()
    {
        using var buffer = CreateBuffer();
        var createInfo = new ResourceMappingCreateInfo()
        {
            Entries =
            [
                new ResourceMappingEntry()
                {
                    Name = "test",
                    Object = buffer,
                }
            ]
        };
        using var resMapping = Device.CreateResourceMapping(createInfo);
        var resource = resMapping.GetResource("test");
        Assert.That(resource, Is.Not.Null);
        Assert.That(resource, Is.EqualTo(buffer));
    }
}
namespace Diligent.Tests;

[TestFixture]
[NonParallelizable]
public class SamplerTest : BaseRenderTest
{
    [Test]
    public void MustCreateSampler()
    {
        var samplerDesc = new SamplerDesc()
        {
            Name = "Test Sampler",
            MinFilter = FilterType.Anisotropic,
            MagFilter = FilterType.Anisotropic,
            MipFilter = FilterType.Anisotropic,
            AddressU = TextureAddressMode.Mirror,
            AddressW = TextureAddressMode.Mirror,
            AddressV = TextureAddressMode.Mirror
        };
        using var sampler = Device.CreateSampler(samplerDesc);
        Assert.That(sampler, Is.Not.Null);
    }

    [Test]
    public void MustGetDesc()
    {
        var samplerDesc = new SamplerDesc()
        {
            Name = "Test Sampler",
            MinFilter = FilterType.Anisotropic,
            MagFilter = FilterType.Anisotropic,
            MipFilter = FilterType.Anisotropic,
            AddressU = TextureAddressMode.Mirror,
            AddressW = TextureAddressMode.Mirror,
            AddressV = TextureAddressMode.Mirror
        };
        using var sampler = Device.CreateSampler(samplerDesc);
        var targetDesc = sampler.Desc;
        
        Assert.Multiple(() =>
        {
            Assert.That(targetDesc.Name, Is.EqualTo(samplerDesc.Name));
            Assert.That(targetDesc.MinFilter, Is.EqualTo(samplerDesc.MinFilter));
            Assert.That(targetDesc.MagFilter, Is.EqualTo(samplerDesc.MagFilter));
        });
    }
}
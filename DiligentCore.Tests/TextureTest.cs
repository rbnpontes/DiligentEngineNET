namespace Diligent.Tests;

[TestFixture]
[NonParallelizable]
public class TextureTest : BaseRenderTest
{
    [Test]
    public void MustCreateTexture()
    {
        var textureDesc = new TextureDesc()
        {
            Name = "Test Texture",
            Width = 1024,
            Height = 1024,
            BindFlags = BindFlags.ShaderResource,
            Usage = Usage.Dynamic,
            Format = TextureFormat.Rgba8Unorm,
            Type = ResourceDimension.Tex2d,
            CPUAccessFlags = CpuAccessFlags.Write,
        };
        using var tex = Device.CreateTexture(textureDesc);
        Assert.That(tex, Is.Not.Null);
    }

    [Test]
    public void MustGetTextureDesc()
    {
        var textureDesc = new TextureDesc()
        {
            Name = "Test Texture",
            Width = 1024,
            Height = 1024,
            BindFlags = BindFlags.ShaderResource,
            Usage = Usage.Dynamic,
            Format = TextureFormat.Rgba8Unorm,
            Type = ResourceDimension.Tex2d,
            CPUAccessFlags = CpuAccessFlags.Write,
        };
        using var tex = Device.CreateTexture(textureDesc);
        var testDesc = tex.Desc;
        Assert.Multiple(() =>
        {
            Assert.That(testDesc.Name, Is.EqualTo(textureDesc.Name));
            Assert.That(testDesc.Width, Is.EqualTo(textureDesc.Width));
            Assert.That(testDesc.Height, Is.EqualTo(textureDesc.Height));
        });
    }

    [Test]
    public void MustGetDefaultView()
    {
        var textureDesc = new TextureDesc()
        {
            Name = "Test Texture",
            Width = 1024,
            Height = 1024,
            BindFlags = BindFlags.ShaderResource,
            Usage = Usage.Dynamic,
            Format = TextureFormat.Rgba8Unorm,
            Type = ResourceDimension.Tex2d,
            CPUAccessFlags = CpuAccessFlags.Write,
        };
        using var tex = Device.CreateTexture(textureDesc);
        using var view = tex.GetDefaultView(TextureViewType.ShaderResource);
        
        Assert.That(view, Is.Not.Null);
    }

    [Test]
    public void MustCreateView()
    {
        var textureDesc = new TextureDesc()
        {
            Name = "Test Texture",
            Width = 1024,
            Height = 1024,
            BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
            Usage = Usage.Default,
            Format = TextureFormat.Rgba8Unorm,
            Type = ResourceDimension.Tex2d,
        };
        using var tex = Device.CreateTexture(textureDesc);
        using var view = tex.CreateView(new TextureViewDesc()
        {
            Name = "Test Texture View",
            Format = TextureFormat.Rgba8Unorm,
            TextureDim = ResourceDimension.Tex2d,
            ViewType = TextureViewType.ShaderResource
        });
        
        Assert.That(view, Is.Not.Null);
    }
}
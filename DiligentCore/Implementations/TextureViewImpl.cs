namespace Diligent;

internal partial class TextureView : ITextureView
{
    public TextureView(IntPtr handle) : base(handle){}
}

internal class UnDisposableTextureView : TextureView
{
    public UnDisposableTextureView(IntPtr handle) : base(handle){}

    protected override void Release()
    {
        // do nothing.
    }
}
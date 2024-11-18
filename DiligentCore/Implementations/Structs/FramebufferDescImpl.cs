namespace Diligent;

public partial class FramebufferDesc
{
    private ITextureView[] _attachments = [];

    public ITextureView[] Attachments
    {
        get => _attachments;
        set
        {
            _attachments = value;
            _data.AttachmentCount = (uint)value.Length;
        }
    }
}
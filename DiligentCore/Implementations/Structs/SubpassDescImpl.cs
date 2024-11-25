namespace Diligent;

public partial class SubpassDesc
{
    private AttachmentReference[] _resolveAttachments = [];
    public AttachmentReference[] ResolveAttachments
    {
        get => _resolveAttachments;
        set
        {
            if (value.Length == 0)
            {
                _resolveAttachments = value;
                return;
            }

            if (value.Length != RenderTargetAttachments.Length)
                throw new Exception(
                    $"{nameof(ResolveAttachments)} value length must be the same size of {nameof(RenderTargetAttachments)} length. Current Length = {value.Length}, Expected Length = {RenderTargetAttachments.Length}");

            _resolveAttachments = value;
        }
    }
    
    public AttachmentReference? DepthStencilAttachment { get; set; }
    public ShadingRateAttachment? ShadingRateAttachment { get; set; }
}
using Diligent.Utils;

namespace Diligent;

internal partial class Framebuffer(IntPtr handle) : DeviceObject(handle), IFramebuffer
{
    public new FramebufferDesc Desc => DiligentDescFactory.GetFramebufferDesc(
        Interop.framebuffer_get_desc(Handle)
    );
}
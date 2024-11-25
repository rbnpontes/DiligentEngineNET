using Diligent.Utils;

namespace Diligent;

internal partial class RenderPass(IntPtr handle) : DeviceObject(handle), IRenderPass
{
    public new RenderPassDesc Desc => DiligentDescFactory.GetRenderPassDesc(
        Interop.render_pass_get_desc(Handle)    
    );
}
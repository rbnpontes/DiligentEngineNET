namespace Diligent;

public interface IRenderPass : IDiligentObject
{
    new RenderPassDesc Desc { get; }
}
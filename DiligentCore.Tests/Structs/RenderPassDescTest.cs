using Diligent.Utils;

namespace Diligent.Tests.Structs;

[TestFixture]
public class RenderPassDescTest
{
    private readonly int _expectedRenderPassSize = 56 // render pass desc size (56)
                                                   + 16 * 2 // attachments size (16)
                                                   + 72 // subpass size (72)
                                                   + 8 * 2 // input attachments size (8)
                                                   + 8 * 2 // render target attachments size (8)
                                                   + 8 * 2 // resolve attachments size (8)
                                                   + 8 // depth stencil attachment size (8)
                                                   + 16 // shading rate attachment size (16)
                                                   + 24 // sub pass dependency size (24)
                                                   + "Test Word".Length + 1;

    private RenderPassDesc CreateRenderPassDesc()
    {
        return new RenderPassDesc()
        {
            Name = "Test Word",
            Attachments =
            [
                new RenderPassAttachmentDesc()
                {
                    Format = TextureFormat.A8Unorm,
                    LoadOp = AttachmentLoadOp.Discard
                },
                new RenderPassAttachmentDesc()
                {
                    Format = TextureFormat.Bc1Unorm,
                    LoadOp = AttachmentLoadOp.Clear
                }
            ],
            Subpasses =
            [
                new SubpassDesc()
                {
                    InputAttachments =
                    [
                        new AttachmentReference()
                        {
                            State = ResourceState.ConstantBuffer
                        },
                        new AttachmentReference()
                        {
                            State = ResourceState.DepthRead
                        }
                    ],
                    RenderTargetAttachments =
                    [
                        new AttachmentReference()
                        {
                            State = ResourceState.Present,
                        },
                        new AttachmentReference()
                        {
                            State = ResourceState.DepthRead
                        }
                    ],
                    ResolveAttachments =
                    [
                        new AttachmentReference()
                        {
                            State = ResourceState.DepthRead
                        },
                        new AttachmentReference()
                        {
                            State = ResourceState.DepthRead
                        }
                    ],
                    DepthStencilAttachment = new AttachmentReference()
                    {
                        State = ResourceState.IndexBuffer
                    },
                    ShadingRateAttachment = new ShadingRateAttachment()
                    {
                        Attachment = new AttachmentReference()
                        {
                            State = ResourceState.InputAttachment
                        }
                    }
                }
            ],
            Dependencies =
            [
                new SubpassDependencyDesc()
                {
                    DstSubpass = 10,
                    SrcSubpass = 20
                }
            ]
        };
    }

    [Test]
    public void ShouldCallCorrectStructSize()
    {
        var desc = CreateRenderPassDesc();
        var size = DiligentDescFactory.GetRenderPassDescSize(desc);

        Assert.That(size, Is.EqualTo(_expectedRenderPassSize));
    }

    [Test]
    public void ShouldGetDataBytes()
    {
        var desc = CreateRenderPassDesc();
        var data = DiligentDescFactory.GetRenderPassDescBytes(desc);

        Assert.That(data, Has.Length.EqualTo(_expectedRenderPassSize));
    }

    [Test]
    public unsafe void ShouldSerializeCorrectly()
    {
        var desc = CreateRenderPassDesc();
        var data = DiligentDescFactory.GetRenderPassDescBytes(desc);
        RenderPassDesc serializedDesc;

        fixed (void* dataPtr = data)
            serializedDesc = DiligentDescFactory.GetRenderPassDesc(new IntPtr(dataPtr));

        Assert.Multiple(() =>
        {
            Assert.That(serializedDesc.Name, Is.EqualTo(desc.Name));

            Assert.That(serializedDesc.Attachments, Has.Length.EqualTo(desc.Attachments.Length));
            Assert.That(serializedDesc.Subpasses, Has.Length.EqualTo(desc.Subpasses.Length));
            Assert.That(serializedDesc.Dependencies, Has.Length.EqualTo(desc.Dependencies.Length));
        });

        for (var i = 0; i < serializedDesc.Attachments.Length; ++i)
        {
            Assert.Multiple(() =>
            {
                Assert.That(serializedDesc.Attachments[i].Format, Is.EqualTo(desc.Attachments[i].Format));
                Assert.That(serializedDesc.Attachments[i].LoadOp, Is.EqualTo(desc.Attachments[i].LoadOp));
            });
        }

        for (var i = 0; i < serializedDesc.Subpasses.Length; ++i)
        {
            Assert.Multiple(() =>
            {
                var subPass = serializedDesc.Subpasses[i];
                var expectedSubPass = desc.Subpasses[i];

                Assert.That(subPass.DepthStencilAttachment?.State,
                    Is.EqualTo(expectedSubPass.DepthStencilAttachment?.State));
                Assert.That(subPass.ShadingRateAttachment?.Attachment.State,
                    Is.EqualTo(expectedSubPass.ShadingRateAttachment?.Attachment.State));

                Assert.That(subPass.InputAttachments,
                    Has.Length.EqualTo(expectedSubPass.InputAttachments.Length));
                Assert.That(subPass.RenderTargetAttachments,
                    Has.Length.EqualTo(expectedSubPass.RenderTargetAttachments.Length));
                Assert.That(subPass.ResolveAttachments,
                    Has.Length.EqualTo(expectedSubPass.ResolveAttachments.Length));

                for (var j = 0; j < subPass.InputAttachments.Length; ++j)
                {
                    var inputAttachment = subPass.InputAttachments[j];
                    var expectedInputAttachment = expectedSubPass.InputAttachments[j];
                    
                    Assert.That(inputAttachment.State, Is.EqualTo(expectedInputAttachment.State));
                }
                
                for (var j = 0; j < subPass.RenderTargetAttachments.Length; ++j)
                {
                    var renderTargetAttachment = subPass.RenderTargetAttachments[j];
                    var expectedRenderTargetAttachment = expectedSubPass.RenderTargetAttachments[j];
                    
                    Assert.That(renderTargetAttachment.State, Is.EqualTo(expectedRenderTargetAttachment.State));
                }

                for (var j = 0; j < subPass.ResolveAttachments.Length; ++j)
                {
                    var resolveAttachment = subPass.ResolveAttachments[j];
                    var expectedResolveAttachment = expectedSubPass.ResolveAttachments[j];
                    
                    Assert.That(resolveAttachment.State, Is.EqualTo(expectedResolveAttachment.State));
                }
            });
        }

        for (var i = 0; i < serializedDesc.Dependencies.Length; ++i)
        {
            var dep = serializedDesc.Dependencies[i];
            var expectedDep = desc.Dependencies[i];
            Assert.Multiple(() =>
            {
                Assert.That(dep.DstSubpass, Is.EqualTo(expectedDep.DstSubpass));
                Assert.That(dep.SrcSubpass, Is.EqualTo(expectedDep.SrcSubpass));
            });
        }
    }
}
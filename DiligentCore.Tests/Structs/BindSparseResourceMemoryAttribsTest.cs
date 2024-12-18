using System.Runtime.CompilerServices;
using Diligent.Null;
using Diligent.Utils;

namespace Diligent.Tests.Structs;

[TestFixture]
public unsafe class BindSparseResourceMemoryAttribsTest
{
    private readonly BindSparseResourceMemoryAttribs _testAttribs = new()
    {
        BufferBinds = [
            new SparseBufferMemoryBindInfo()
            {
                BufferPtr = 0xFFAAFF,
                Ranges = [
                    new SparseBufferMemoryBindRange()
                    {
                        BufferOffset = 1020,
                        MemoryOffset = 2048,
                        MemorySize = 2424
                    }
                ]
            }
        ],
        TextureBinds = [
            new SparseTextureMemoryBindInfo()
            {
                TexturePtr = 0xDAC0DAC,
                Ranges = [
                    new SparseTextureMemoryBindRange()
                    {
                        MemorySize = 24444,
                        MemoryOffset = 12,
                        MipLevel = 1023
                    }
                ]
            }
        ],
        WaitFences = [
            new NullFence(0xBADF00D),
            new NullFence(0xB00B)
        ],
        WaitFenceValues = [0x10, 0x20],
        SignalFences = [
            new NullFence(0xFAFAFA),
            new NullFence(0xBADBAD)
        ],
        SignalFenceValues = [0x30, 0x40]
    };

    private readonly int _testAttribsSize = 80 // BindSparseResourceMemoryAttribs size 
                                           + 24 // SparseBufferMemoryBindInfo size
                                           + 32 // SparseBufferMemoryBindRange size
                                           + 24 // SparseTextureMemoryBindInfo size
                                           + 64 // SparseTextureMemoryBindRange size
                                           + 4 * Unsafe.SizeOf<ulong>() // wait fence values size
                                           + 4 * Unsafe.SizeOf<IntPtr>(); // fences size

    [Test]
    public void ShouldCalcCorrectSize()
    {
        var size = DiligentDescFactory.GetBindSparseResourceMemoryAttribsSize(_testAttribs);
        Assert.That(size, Is.EqualTo(_testAttribsSize));
    }

    [Test]
    public void ShouldSerializeStruct()
    {
        var data = DiligentDescFactory.GetBindSparseResourceMemoryAttribsBytes(_testAttribs);
        BindSparseResourceMemoryAttribs attribs;
        
        fixed(void* dataPtr = data)
            attribs = DiligentDescFactory.GetBindSparseResourceMemoryAttribs(new IntPtr(dataPtr));
        
        Assert.Multiple(() =>
        {
            Assert.That(attribs.BufferBinds, Has.Length.EqualTo(_testAttribs.BufferBinds.Length));
            Assert.That(attribs.TextureBinds, Has.Length.EqualTo(_testAttribs.TextureBinds.Length));
            Assert.That(attribs.WaitFences, Has.Length.EqualTo(_testAttribs.WaitFences.Length));
            Assert.That(attribs.SignalFences, Has.Length.EqualTo(_testAttribs.SignalFences.Length));
        });
        
        for (var i = 0; i < attribs.BufferBinds.Length; ++i)
        {
            var assertBufferBind = attribs.BufferBinds[i];
            var expectedBufferBind = _testAttribs.BufferBinds[i];
            Assert.Multiple(() =>
            {
                Assert.That(assertBufferBind.BufferPtr, Is.EqualTo(expectedBufferBind.BufferPtr));
                Assert.That(assertBufferBind.Ranges, Has.Length.EqualTo(expectedBufferBind.Ranges.Length));
            });
            for (var j = 0; j < assertBufferBind.Ranges.Length; ++j)
            {
                var assertRange = assertBufferBind.Ranges[j];
                var expectedRange = expectedBufferBind.Ranges[j];
                Assert.Multiple(() =>
                {
                    Assert.That(assertRange.BufferOffset, Is.EqualTo(expectedRange.BufferOffset));
                    Assert.That(assertRange.MemoryOffset, Is.EqualTo(expectedRange.MemoryOffset));
                    Assert.That(assertRange.MemorySize, Is.EqualTo(expectedRange.MemorySize));
                });
            }
        }

        for (var i = 0; i < attribs.TextureBinds.Length; ++i)
        {
            var assertTextureBind = attribs.TextureBinds[i];
            var expectedTextureBind = _testAttribs.TextureBinds[i];
            
            Assert.That(assertTextureBind.TexturePtr, Is.EqualTo(expectedTextureBind.TexturePtr));
            for (var j = 0; j < assertTextureBind.Ranges.Length; ++j)
            {
                var assertRange = assertTextureBind.Ranges[j];
                var expectedRange = expectedTextureBind.Ranges[j];
                Assert.Multiple(() =>
                {
                    Assert.That(assertRange.MemorySize, Is.EqualTo(expectedRange.MemorySize));
                    Assert.That(assertRange.MemoryOffset, Is.EqualTo(expectedRange.MemoryOffset));
                    Assert.That(assertRange.MipLevel, Is.EqualTo(expectedRange.MipLevel));
                });
            }
        }

        for (var i = 0; i < attribs.WaitFences.Length; ++i)
            Assert.That(attribs.WaitFences[i].Handle, Is.EqualTo(_testAttribs.WaitFences[i].Handle));
        
        for (var i = 0; i < attribs.WaitFenceValues.Length; ++i)
            Assert.That(attribs.WaitFenceValues[i], Is.EqualTo(_testAttribs.WaitFenceValues[i]));
        
        for(var i = 0; i < attribs.SignalFences.Length; ++i)
            Assert.That(attribs.SignalFences[i].Handle, Is.EqualTo(_testAttribs.SignalFences[i].Handle));

        Assert.Pass();
    }
}
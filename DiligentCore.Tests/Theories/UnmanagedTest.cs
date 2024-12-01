using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using IntPtr = System.IntPtr;

namespace Diligent.Tests.Theories;

[TestFixture]
public unsafe class UnmanagedTest
{
    struct TestStruct
    {
        public int X;
        public int Y;
    }

    struct TestStruct2
    {
        public int X;
        public int Y;
        public IntPtr Text;
    }

    [Theory]
    public void CouldWriteStructIntoByteArray()
    {
        var byteArray = new byte[Unsafe.SizeOf<TestStruct>()];
        var span = byteArray.AsSpan();
        fixed (void* basePtr = span)
        {
            var structData = (TestStruct*)basePtr;
            *structData = CreateStruct();
        }

        fixed (void* ptr = span)
        {
            var structData = (TestStruct*)ptr;
            var expectedData = CreateStruct();
            Assert.That(structData->X, Is.EqualTo(expectedData.X));
            Assert.That(structData->Y, Is.EqualTo(expectedData.Y));
        }

        TestStruct CreateStruct()
        {
            var result = new TestStruct();
            result.X = 100;
            result.Y = 200;
            return result;
        }
    }

    [Theory]
    public void CanStringStoredSideOfStruct()
    {
        var str = "Test Word";
        var strBytes = Encoding.ASCII.GetBytes(str);

        var byteArray = new byte[Unsafe.SizeOf<TestStruct2>() + str.Length + 2];
        var span = byteArray.AsSpan();
        fixed (void* ptr = span)
        fixed (void* strPtr = strBytes)
        {
            Unsafe.CopyBlockUnaligned(ptr, strPtr, (uint)strBytes.Length);
            var structPtr = IntPtr.Add(new IntPtr(ptr), strBytes.Length + 1);
            (*(TestStruct2*)structPtr) = CreateStruct(new IntPtr(ptr));
        }

        fixed (void* ptr = span)
        {
            var structPtr = (TestStruct2*)IntPtr.Add(new IntPtr(ptr), strBytes.Length + 1);
            var targetStr = Marshal.PtrToStringAnsi(structPtr->Text);
            var targetStruct = CreateStruct(IntPtr.Zero);

            Assert.That(structPtr->X, Is.EqualTo(targetStruct.X));
            Assert.That(structPtr->Y, Is.EqualTo(targetStruct.Y));
            Assert.That(targetStr, Is.EqualTo(str));
        }

        TestStruct2 CreateStruct(IntPtr strAddr)
        {
            var result = new TestStruct2();
            result.X = 0xBADF00D;
            result.Y = 0XBADB00B;
            result.Text = strAddr;
            return result;
        }
    }
}
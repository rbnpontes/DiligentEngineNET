using System.Runtime.InteropServices;

namespace Diligent.Tests;

[TestFixture]
public class EngineFactoryTest : BaseFactoryTest
{
    private struct TestStruct
    {
        public int A;
        public IntPtr B;
    }

    [Test]
    public void MustEnumerateAdapters()
    {
        var engineFactory = GetFactory();
        var adapters = engineFactory.EnumerateAdapters(new Version(11, 0));
        Assert.That(adapters, Has.No.Empty);

        foreach (var adapter in adapters)
            Assert.That(adapter.Description, Is.Not.Empty);
    }


    [DllImport(Constants.LibName, EntryPoint = "exec_debug_message_callback",
        CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr ExecDbgMsgCallback(int severity, IntPtr msgPtr, IntPtr funcPtr,
        IntPtr filePtr, int line);

    [Test]
    public void MustSetMessageCallback()
    {
        var passed = false;
        using var engineFactory = GetFactory();
        DebugMessageCallbackDelegate callback = (severity, message, function, file, line) =>
        {
            Assert.That(severity, Is.EqualTo(DebugMessageSeverity.Info));
            Assert.That(message, Is.EqualTo("Test Message"));
            Assert.That(function, Is.EqualTo(nameof(MustSetMessageCallback)));
            Assert.That(file, Is.EqualTo("EngineFactoryTest.cs"));
            Assert.That(line, Is.EqualTo(1234));
            passed = true;
        };
        engineFactory.SetMessageCallback(callback);

        var stringPointers = new[]
        {
            Marshal.StringToHGlobalAnsi("Test Message"),
            Marshal.StringToHGlobalAnsi(nameof(MustSetMessageCallback)),
            Marshal.StringToHGlobalAnsi("EngineFactoryTest.cs"),
        };

        // do unmanaged call
        ExecDbgMsgCallback((int)DebugMessageSeverity.Info, stringPointers[0], stringPointers[1],
            stringPointers[2], 1234);

        Assert.That(passed, Is.True);

        // free resources
        foreach (var ptr in stringPointers)
            Marshal.FreeHGlobal(ptr);

        Assert.Pass();
    }

    [Test]
    public void MustGetApiInfo()
    {
        var engineFactory = GetFactory();
        var apiInfo = engineFactory.APIInfo;

        Assert.That(apiInfo, Is.Not.Null);
    }

    [Test]
    public void MustSetBreakOnError()
    {
        var engineFactory = GetFactory();
        engineFactory.SetBreakOnError(true);
        engineFactory.SetBreakOnError(false);
    }

    [Test]
    public void MustCreateDefaultShaderSourceStreamFactory()
    {
        var engineFactory = GetFactory();
        using (engineFactory.CreateDefaultShaderSourceStreamFactory(Environment.CurrentDirectory))
        {
        }

        Assert.Pass();
    }

    [Test]
    public void MustCreateDataBlob()
    {
        var factory = GetFactory();
        var dataBlob = factory.CreateDataBlob(1024);
        Assert.That(dataBlob, Is.Not.Null);
        dataBlob.Dispose();

        var values = new[] { 1, 2, 3, 4 };
        dataBlob = factory.CreateDataBlob<int>(values);
        Assert.That(dataBlob, Is.Not.Null);
        dataBlob.Dispose();

        var data = new TestStruct();
        data.A = 20;
        data.B = new IntPtr(0xFF);
        dataBlob = factory.CreateDataBlob(ref data);
        Assert.That(dataBlob, Is.Not.Null);
        dataBlob.Dispose();
    }
}
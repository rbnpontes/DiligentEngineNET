using System.Runtime.InteropServices;
using Diligent.Utils;

namespace Diligent;

internal abstract class MessageCallbackHandler
{
    protected DebugMessageCallbackDelegate? mCallback;

    public void SetCallback(DebugMessageCallbackDelegate? callback)
    {
        mCallback = callback;
    }
}

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal delegate void NativeDebugMessageCallbackDelegate(DebugMessageSeverity severity, IntPtr message, IntPtr function,
    IntPtr file, int line);
internal class DefaultMessageCallbackHandler : MessageCallbackHandler
{
    private NativeDebugMessageCallbackDelegate _internalCallback;
    private IntPtr _nativeDelegatePtr;
    public DefaultMessageCallbackHandler(IntPtr factoryHandle)
    {
        _internalCallback = OnMessageCallback;
        _nativeDelegatePtr = Marshal.GetFunctionPointerForDelegate(_internalCallback);
        EngineFactory.Interop.engine_factory_set_message_callback(factoryHandle, _nativeDelegatePtr);
    }

    private void OnMessageCallback(DebugMessageSeverity severity, IntPtr message, IntPtr function,
        IntPtr file, int line)
    {
        if (mCallback is null)
            return;
        
        var messageStr = Marshal.PtrToStringAnsi(message) ?? string.Empty;
        var funcStr = Marshal.PtrToStringAnsi(function) ?? string.Empty;
        var fileStr = Marshal.PtrToStringAnsi(file) ?? string.Empty;
        
        mCallback(severity, messageStr, funcStr, fileStr, line);
    }
}

internal class WebMessageCallbackHandler : MessageCallbackHandler
{
    private IntPtr _nativeDelegatePtr;
    public WebMessageCallbackHandler(IntPtr factoryHandle)
    {
        _nativeDelegatePtr = WebInteropUtils.DelegateRegister(OnMessage, "viiiii");
        EngineFactory.WebInterop.engine_factory_set_message_callback(factoryHandle, _nativeDelegatePtr);
    }

    private void OnMessage(int objIdx)
    {
        if(mCallback is null)
            return;
        
        var severity = JsObjectUtils.ArrayGetInt(objIdx, 0);
        var msgPtr = JsObjectUtils.ArrayGetPtr(objIdx, 1);
        var funcPtr = JsObjectUtils.ArrayGetPtr(objIdx, 2);
        var filePtr = JsObjectUtils.ArrayGetPtr(objIdx, 3);
        var line = JsObjectUtils.ArrayGetInt(objIdx, 4);
        
        var msg = msgPtr == IntPtr.Zero ? string.Empty : WebInteropUtils.GetString(msgPtr);
        var func = funcPtr == IntPtr.Zero ? string.Empty : WebInteropUtils.GetString(funcPtr);
        var file = filePtr == IntPtr.Zero ? string.Empty : WebInteropUtils.GetString(filePtr);
        
        mCallback((DebugMessageSeverity)severity, msg, func, file, line);
    }
}
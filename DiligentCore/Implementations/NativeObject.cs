namespace Diligent;

public class NativeObject : INativeObject
{
    private IntPtr _handle;

    public IntPtr Handle => GetCurrentHandle();

    internal NativeObject(IntPtr handle)
    {
        _handle = handle;
    }

    protected virtual IntPtr GetCurrentHandle()
    {
        return _handle;
    }

    protected virtual void SetCurrentHandle(IntPtr handle)
    {
        _handle = handle;
    }
}
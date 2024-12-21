namespace Diligent;

public class NativeObject : INativeObject
{
    protected IntPtr InternalHandle { get; private set; }

    public IntPtr Handle => GetCurrentHandle();

    
    internal NativeObject(IntPtr handle)
    {
        InternalHandle = handle;
    }

    protected virtual IntPtr GetCurrentHandle()
    {
        return InternalHandle;
    }

    protected virtual void SetCurrentHandle(IntPtr handle)
    {
        InternalHandle = handle;
    }
}
namespace Diligent;

public class NativeObject : INativeObject
{
    public IntPtr Handle { get; internal set; }
    internal NativeObject(IntPtr handle)
    {
        Handle = handle;
    }

    protected void AddToRegister()
    {
        NativeObjectRegistry.AddToRegister(this);
    }
}
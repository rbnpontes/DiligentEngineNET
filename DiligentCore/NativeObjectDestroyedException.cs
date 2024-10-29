namespace Diligent;

public class NativeObjectDestroyedException(Type type) : Exception($"NativeObject '{type.Name}' has been destroyed.")
{
}
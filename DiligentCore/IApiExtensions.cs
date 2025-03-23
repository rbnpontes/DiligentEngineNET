namespace Diligent;

internal interface IApiExtensions
{
    public int GetApiVersion();
    public void SetReleaseCallback(Action<nint, nint> releaseCallback);
}
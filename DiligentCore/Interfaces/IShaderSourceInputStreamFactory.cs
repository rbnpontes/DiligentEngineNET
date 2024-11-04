namespace Diligent;

public interface IShaderSourceInputStreamFactory : IDiligentObject
{
    IFileStream CreateInputStream(string name);
    IFileStream CreateInputStream(string name, CreateShaderSourceInputStreamFlags flags);
}
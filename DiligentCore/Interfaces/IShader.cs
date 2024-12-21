namespace Diligent;

public interface IShader : IDiligentObject
{
    uint ResourceCount { get; }
    ShaderResourceDesc GetResourceDesc(uint index);
    ShaderCodeBufferDesc GetConstantBufferDesc(uint index);
    byte[] GetBytecode();
    ShaderStatus GetStatus(bool waitForCompletion = false);
}
namespace Diligent;

public interface IShaderBindingTable : IDeviceObject
{
    new ShaderBindingTableDesc Desc { get; }
    bool Verify(VerifySbtFlags flags);
    void Reset(IPipelineState pipelineState);
    void ResetHitGroups();
    void BindRayGenShader(string shaderGroupName, IntPtr data, uint dataSize);
    void BindRayGenShader(string shaderGroupName, byte[] data);
    void BindMissShader(string shaderGroupName, uint missIndex, IntPtr data, uint dataSize);
    void BindMissShader(string shaderGroupName, uint missIndex, byte[] data);

    void BindHitGroupForGeometry(ITopLevelAS tlas, string instanceName, string geometryName,
        uint rayOffsetInHitGroupIndex, string shaderGroupName, IntPtr data, uint dataSize);

    void BindHitGroupForGeometry(ITopLevelAS tlas, string instanceName, string geometryName,
        uint rayOffsetInHitGroupIndex, string shaderGroupName, byte[] data);

    void BindHitGroupByIndex(uint bindingIndex, string shaderGroupName, IntPtr data, uint dataSize);
    void BindHitGroupByIndex(uint bindingIndex, string shaderGroupName, byte[] data);

    void BindHitGroupForInstance(ITopLevelAS tlas, string instanceName, uint rayOffsetInHitGroupIndex,
        string shaderGroupName, IntPtr data, uint dataSize);
    void BindHitGroupForInstance(ITopLevelAS tlas, string instanceName, uint rayOffsetInHitGroupIndex,
        string shaderGroupName, byte[] data);
    void BindHitGroupForTLAS(ITopLevelAS tlas, uint rayOffsetInHitGroupIndex, string shaderGroupName, IntPtr data,
        uint dataSize);
    void BindHitGroupForTLAS(ITopLevelAS tlas, uint rayOffsetInHitGroupIndex, string shaderGroupName, byte[] data);
    void BindCallableShader(string name, uint callableIndex, IntPtr data, uint dataSize);
    void BindCallableShader(string name, uint callableIndex, byte[] data);
}
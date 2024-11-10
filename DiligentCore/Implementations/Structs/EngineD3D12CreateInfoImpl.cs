namespace Diligent;

public partial class EngineD3D12CreateInfo
{
    public string D3D12DllName { get; set; } = "d3d12.dll";
    public string DxCompilerPath { get; set; } = string.Empty;
    public unsafe EngineD3D12CreateInfo()
    {
        _data.D3D12ValidationFlags = D3d12ValidationFlags.D3d12ValidationFlagBreakOnCorruption;
        
        _data.CPUDescriptorHeapAllocationSize[0] = 8192;
        _data.CPUDescriptorHeapAllocationSize[1] = 2048;
        _data.CPUDescriptorHeapAllocationSize[2] = 1024;
        _data.CPUDescriptorHeapAllocationSize[3] = 1024;
        
        _data.GPUDescriptorHeapSize[0] = 16384;
        _data.GPUDescriptorHeapSize[1] = 1024;
        
        _data.GPUDescriptorHeapDynamicSize[0] = 8192;
        _data.GPUDescriptorHeapDynamicSize[1] = 1024;
        
        _data.DynamicDescriptorAllocationChunkSize[0] = 256;
        _data.DynamicDescriptorAllocationChunkSize[1] = 32;
        
        _data.DynamicHeapPageSize = 1 << 20;
        _data.NumDynamicHeapPagesToReserve = 1;
        
        _data.QueryPoolSizes[0] = 0;
        _data.QueryPoolSizes[1] = 128;
        _data.QueryPoolSizes[2] = 128;
        _data.QueryPoolSizes[3] = 512;
        _data.QueryPoolSizes[4] = 128;
        _data.QueryPoolSizes[5] = 256;
    }

    public override void SetValidation(ValidationLevel level)
    {
        base.SetValidation(level);

        _data.D3D12ValidationFlags = D3d12ValidationFlags.D3d12ValidationFlagNone;
        if (level >= ValidationLevel.ValidationLevel1)
            _data.D3D12ValidationFlags |= D3d12ValidationFlags.D3d12ValidationFlagBreakOnCorruption;
        if (level >= ValidationLevel.ValidationLevel2)
            _data.D3D12ValidationFlags |= D3d12ValidationFlags.D3d12ValidationFlagEnableGpuBasedValidation;
    }
}
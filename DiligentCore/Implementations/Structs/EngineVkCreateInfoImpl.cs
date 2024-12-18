namespace Diligent;

public partial class EngineVkCreateInfo
{
    public unsafe EngineVkCreateInfo() : base()
    {
        MainDescriptorPoolSize.MaxDescriptorSets = 8192;
        MainDescriptorPoolSize.NumSeparateSamplerDescriptors = 1024;
        MainDescriptorPoolSize.NumCombinedSamplerDescriptors = 8192;
        MainDescriptorPoolSize.NumSampledImageDescriptors = 8192;
        MainDescriptorPoolSize.NumStorageImageDescriptors = 1024;
        MainDescriptorPoolSize.NumUniformBufferDescriptors = 4096;
        MainDescriptorPoolSize.NumStorageBufferDescriptors = 4096;
        MainDescriptorPoolSize.NumUniformTexelBufferDescriptors = 1024;
        MainDescriptorPoolSize.NumStorageTexelBufferDescriptors = 1024;
        MainDescriptorPoolSize.NumInputAttachmentDescriptors = 256;
        MainDescriptorPoolSize.NumAccelStructDescriptors = 256;
        
        DynamicDescriptorPoolSize.MaxDescriptorSets = 2048;
        DynamicDescriptorPoolSize.NumSeparateSamplerDescriptors = 256;
        DynamicDescriptorPoolSize.NumCombinedSamplerDescriptors = 2048;
        DynamicDescriptorPoolSize.NumSampledImageDescriptors = 2048;
        DynamicDescriptorPoolSize.NumStorageImageDescriptors = 256;
        DynamicDescriptorPoolSize.NumUniformBufferDescriptors = 1024;
        DynamicDescriptorPoolSize.NumStorageBufferDescriptors = 1024;
        DynamicDescriptorPoolSize.NumUniformTexelBufferDescriptors = 256;
        DynamicDescriptorPoolSize.NumStorageTexelBufferDescriptors = 256;
        DynamicDescriptorPoolSize.NumInputAttachmentDescriptors = 64;
        DynamicDescriptorPoolSize.NumAccelStructDescriptors = 64;
        
        _data.DeviceLocalMemoryPageSize = 16 << 20;
        _data.HostVisibleMemoryPageSize = 16 << 20;
        _data.DeviceLocalMemoryReserveSize = 256 << 20;
        _data.HostVisibleMemoryReserveSize = 256 << 20;
        _data.UploadHeapPageSize = 1 << 20;
        _data.DynamicHeapSize = 8 << 20;
        _data.DynamicHeapPageSize = 256 << 10;
        _data.QueryPoolSizes[0] = 0;
        _data.QueryPoolSizes[1] = 128;
        _data.QueryPoolSizes[2] = 128;
        _data.QueryPoolSizes[3] = 512;
        _data.QueryPoolSizes[4] = 128;
        _data.QueryPoolSizes[5] = 256;
    }
}
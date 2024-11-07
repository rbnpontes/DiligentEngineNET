namespace Diligent;

public partial class EngineVkCreateInfo
{
    public unsafe EngineVkCreateInfo() : base()
    {
        _data.MainDescriptorPoolSize.MaxDescriptorSets = 8192;
        _data.MainDescriptorPoolSize.NumSeparateSamplerDescriptors = 1024;
        _data.MainDescriptorPoolSize.NumCombinedSamplerDescriptors = 8192;
        _data.MainDescriptorPoolSize.NumSampledImageDescriptors = 8192;
        _data.MainDescriptorPoolSize.NumStorageImageDescriptors = 1024;
        _data.MainDescriptorPoolSize.NumUniformBufferDescriptors = 4096;
        _data.MainDescriptorPoolSize.NumStorageBufferDescriptors = 4096;
        _data.MainDescriptorPoolSize.NumUniformTexelBufferDescriptors = 1024;
        _data.MainDescriptorPoolSize.NumStorageTexelBufferDescriptors = 1024;
        _data.MainDescriptorPoolSize.NumInputAttachmentDescriptors = 256;
        _data.MainDescriptorPoolSize.NumAccelStructDescriptors = 256;
        
        _data.DynamicDescriptorPoolSize.MaxDescriptorSets = 2048;
        _data.DynamicDescriptorPoolSize.NumSeparateSamplerDescriptors = 256;
        _data.DynamicDescriptorPoolSize.NumCombinedSamplerDescriptors = 2048;
        _data.DynamicDescriptorPoolSize.NumSampledImageDescriptors = 2048;
        _data.DynamicDescriptorPoolSize.NumStorageImageDescriptors = 256;
        _data.DynamicDescriptorPoolSize.NumUniformBufferDescriptors = 1024;
        _data.DynamicDescriptorPoolSize.NumStorageBufferDescriptors = 1024;
        _data.DynamicDescriptorPoolSize.NumUniformTexelBufferDescriptors = 256;
        _data.DynamicDescriptorPoolSize.NumStorageTexelBufferDescriptors = 256;
        _data.DynamicDescriptorPoolSize.NumInputAttachmentDescriptors = 64;
        _data.DynamicDescriptorPoolSize.NumAccelStructDescriptors = 64;
        
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
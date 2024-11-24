using Diligent.Utils;

namespace Diligent;

internal unsafe partial class ResourceMapping(IntPtr handle) : DiligentObject(handle), IResourceMapping
{
    public ulong Size => Interop.resource_mapping_get_size(Handle);

    public void AddResource(string name, IDeviceObject obj, bool isUnique)
    {
        using var strAlloc = new StringAllocator();
        Interop.resource_mapping_add_resource(Handle,
            strAlloc.Acquire(name),
            obj.Handle,
            isUnique);
    }

    public void AddResourceArray(string name, uint startIndex, IDeviceObject[] objects, bool isUnique)
    {
        using var strAlloc = new StringAllocator();
        var objectsData = objects.Select(x => x.Handle)
            .ToArray()
            .AsSpan();

        fixed (void* objectsPtr = objectsData)
        {
            Interop.resource_mapping_add_resource_array(Handle,
                strAlloc.Acquire(name),
                startIndex,
                new IntPtr(objectsPtr),
                (uint)objects.Length,
                isUnique);
        }
    }

    public void RemoveResourceByName(string name, uint arrayIndex = 0)
    {
        using var strAlloc = new StringAllocator();
        Interop.resource_mapping_remove_resource_by_name(Handle,
            strAlloc.Acquire(name),
            arrayIndex);
    }

    public IDeviceObject? GetResource(string name, uint arrayIndex = 0)
    {
        using var strAlloc = new StringAllocator();
        var resourcePtr = Interop.resource_mapping_get_resource(Handle,
            strAlloc.Acquire(name),
            arrayIndex);
        return DiligentObjectsFactory.TryGetOrCreateDeviceObject(resourcePtr);
    }
}
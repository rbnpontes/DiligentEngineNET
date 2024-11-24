namespace Diligent;

public interface IResourceMapping : IDiligentObject
{
     ulong Size { get; }
     void AddResource(string name, IDeviceObject obj, bool isUnique);
     void AddResourceArray(string name, uint startIndex, IDeviceObject[] objects, bool isUnique);
     void RemoveResourceByName(string name, uint arrayIndex = 0);
     IDeviceObject? GetResource(string name, uint arrayIndex = 0);
}
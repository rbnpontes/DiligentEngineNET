namespace Diligent;

public partial class ResourceMappingCreateInfo
{
    private ResourceMappingEntry[] _entries = [];
    public ResourceMappingEntry[] Entries
    {
        get => _entries;
        set
        {
            _entries = value;
            _data.NumEntries = (uint)value.Length;
        }
    }
}
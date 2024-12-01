namespace Diligent;

public partial class MultiDrawIndexedAttribs
{
    private MultiDrawIndexedItem[] _drawItems = [];
    public MultiDrawIndexedItem[] DrawItems
    {
        get => _drawItems;
        set
        {
            _data.DrawCount = (uint)value.Length;
            _drawItems = value;
        }
    }

    public MultiDrawIndexedAttribs()
    {
        _data.NumInstances = 1;
    }
}
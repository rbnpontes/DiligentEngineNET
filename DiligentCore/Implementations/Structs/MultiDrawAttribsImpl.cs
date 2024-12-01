namespace Diligent;

public partial class MultiDrawAttribs
{
    private MultiDrawItem[] _drawItems = [];

    public MultiDrawItem[] DrawItems
    {
        get => _drawItems;
        set
        {
            _data.DrawCount = (uint)value.Length;
            _drawItems = value;
        }
    }
    
    public MultiDrawAttribs()
    {
        _data.NumInstances = 1;
    }
}
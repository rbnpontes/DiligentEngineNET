namespace Diligent;

public partial class InputLayoutDesc
{
    private LayoutElement[] _layoutElements = [];
    public LayoutElement[] LayoutElements
    {
        get => _layoutElements;
        set
        {
            _layoutElements = value;
            _data.NumElements = (uint)value.Length;
        }
    }
}
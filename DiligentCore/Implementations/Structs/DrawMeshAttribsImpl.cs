namespace Diligent;

public partial class DrawMeshAttribs
{
    public DrawMeshAttribs()
    {
        _data.ThreadGroupCountX =
            _data.ThreadGroupCountY =
                _data.ThreadGroupCountZ = 1;
    }
}
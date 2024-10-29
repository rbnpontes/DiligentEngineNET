namespace Diligent;

public partial class Version
{
    public Version() {}

    public Version(uint major, uint minor)
    {
        _data.Major = major;
        _data.Minor = minor;
    }
}
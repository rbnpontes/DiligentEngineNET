namespace Diligent;

public partial class LayoutElement
{
    public LayoutElement()
    {
        HLSLSemantic = "ATTRIB";
        _data.ValueType = ValueType.Float32;
        _data.IsNormalized = true;
        _data.RelativeOffset = Constants.LayoutElementAutoOffset;
        _data.Stride = Constants.LayoutElementAutoStride;
        _data.Frequency = InputElementFrequency.PerVertex;
        _data.InstanceDataStepRate = 1;
    }
}
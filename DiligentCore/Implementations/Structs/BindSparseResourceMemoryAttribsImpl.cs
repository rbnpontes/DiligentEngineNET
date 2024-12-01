namespace Diligent;

public partial class BindSparseResourceMemoryAttribs
{
    private ulong[] _waitFenceValues = [];
    private ulong[] _signalFenceValues = [];

    public ulong[] WaitFenceValues
    {
        get => _waitFenceValues;
        set
        {
            if (value.Length != WaitFences.Length)
                throw new Exception(
                    $"{nameof(WaitFenceValues)} length must be the same size of {nameof(WaitFences)}. Current length = {value.Length} | {nameof(WaitFences)} length = {WaitFences.Length}");
            _waitFenceValues = value;
        }
    }

    public ulong[] SignalFenceValues
    {
        get => _signalFenceValues;
        set
        {
            if(value.Length != SignalFences.Length)
                throw new Exception(
                    $"{nameof(SignalFenceValues)} length must be the same size of {nameof(SignalFences)}. Current length = {value.Length} | {nameof(SignalFences)} length = {SignalFences.Length}");
            _signalFenceValues = value;
        }
    }
}
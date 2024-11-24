using System.Runtime.InteropServices;

namespace Diligent;

internal unsafe partial class Sampler : ISampler
{
    public new SamplerDesc Desc
    {
        get
        {
            var data = (SamplerDesc.__Internal*)Interop.sampler_get_desc(Handle);
            var result = SamplerDesc.FromInternalStruct(*data);
            result.Name = Marshal.PtrToStringAnsi(data->Name) ?? string.Empty;
            return result;
        }
    }
    public Sampler(IntPtr handle) : base(handle) {}
}

internal class UnDisposableSampler(IntPtr handle) : Sampler(handle)
{
    protected override void Release()
    {
    }
}
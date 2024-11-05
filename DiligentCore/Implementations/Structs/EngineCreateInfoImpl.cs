namespace Diligent;

public partial class EngineCreateInfo
{
     public EngineCreateInfo()
     {
          _data.EngineAPIVersion = Constants.DiligentApiVersion;
          _data.AdapterId = Constants.DefaultAdapterId;
          _data.GraphicsAPIVersion = Version.GetInternalStruct(new Version());
     }
}
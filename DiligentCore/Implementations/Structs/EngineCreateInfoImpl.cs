namespace Diligent;

public partial class EngineCreateInfo
{
     public EngineCreateInfo()
     {
          _data.EngineAPIVersion = Constants.DiligentApiVersion;
          _data.AdapterId = Constants.DefaultAdapterId;
          _data.GraphicsAPIVersion = Version.GetInternalStruct(new Version());
          _data.NumAsyncShaderCompilationThreads = 0xFFFFFFFFu;
     }

     public virtual void SetValidation(ValidationLevel level)
     {
          EnableValidation = level > ValidationLevel.ValidationLevelDisabled;
          ValidationFlags = ValidationFlags.ValidationFlagNone;
          if (level >= ValidationLevel.ValidationLevel1)
               ValidationFlags |= ValidationFlags.ValidationFlagCheckShaderBufferSize;
     }
}
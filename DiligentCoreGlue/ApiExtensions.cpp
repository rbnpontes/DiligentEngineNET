#include <APIInfo.h>
#include <ReleaseCallback.h>
#include "Api.h"

EXPORT int diligent_core_api_ext_get_api_version()
{
    return DILIGENT_API_VERSION;
}

EXPORT void diligent_core_api_set_release_callback(void* callback) 
{
    Diligent::SetReleaseCallback(reinterpret_cast<Diligent::ReleaseCallbackType>(callback));
}
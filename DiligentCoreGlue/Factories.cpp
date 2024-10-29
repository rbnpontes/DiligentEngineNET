#include "Api.h"
#ifdef PLATFORM_WIN32
#include <EngineFactoryD3D11.h>
#include <EngineFactoryD3D12.h>
#endif
#include <EngineFactoryVk.h>
#include <EngineFactoryOpenGL.h>

using namespace Diligent;
EXPORT void* diligent_core_get_d3d11_factory()
{
#ifdef PLATFORM_WIN32
	return 	GetEngineFactoryD3D11();
#else
	return nullptr;
#endif
}

EXPORT void* diligent_core_get_d3d12_factory()
{
#ifdef PLATFORM_WIN32
	return 	GetEngineFactoryD3D12();
#else
	return nullptr;
#endif
}

EXPORT void* diligent_core_get_vk_factory()
{
	return GetEngineFactoryVk();
}

EXPORT void* diligent_core_get_opengl_factory()
{
	return GetEngineFactoryOpenGL();
}
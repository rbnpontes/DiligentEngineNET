#include "Api.h"

#ifdef WEB
typedef void* IntPtr;
typedef unsigned int uint;
typedef unsigned short TextureFormat;

// On web assembly targets, dotnet generates direct link
// to the pinvoke call instead of look for function like
// any system does. for this reason, we will declare
// empty methods to fullfill requirements of dotnet compiler

EXPORT void engine_factory_d3d11_create_device_and_contexts_d3d11(IntPtr _this, IntPtr arg0, IntPtr arg1, IntPtr arg2) {}

EXPORT void engine_factory_d3d11_attach_to_d3d11device(IntPtr _this, IntPtr arg0, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4) {}

EXPORT void engine_factory_d3d11_enumerate_display_modes(IntPtr _this, IntPtr arg0, uint arg1, uint arg2, TextureFormat arg3, IntPtr arg4, IntPtr arg5) {}

EXPORT void engine_factory_d3d12_create_command_queue_d3d12(IntPtr _this, IntPtr arg0, IntPtr arg1, IntPtr arg2, IntPtr arg3) {}

EXPORT void engine_factory_d3d12_create_device_and_contexts_d3d12(IntPtr _this, IntPtr arg0, IntPtr arg1, IntPtr arg2) {}

EXPORT void engine_factory_d3d12_attach_to_d3d12device(IntPtr _this, IntPtr arg0, uint arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5) {}

EXPORT void engine_factory_d3d12_enumerate_display_modes(IntPtr _this, IntPtr arg0, uint arg1, uint arg2, TextureFormat arg3, IntPtr arg4, IntPtr arg5) {}

EXPORT bool engine_factory_d3d12_load_d3d12(IntPtr _this, IntPtr arg0) { return false; }

EXPORT void engine_factory_vk_create_device_and_contexts_vk(IntPtr _this, IntPtr arg0, IntPtr arg1, IntPtr arg2) {}

EXPORT void engine_factory_vk_enable_device_simulation(IntPtr _this) { }
#endif
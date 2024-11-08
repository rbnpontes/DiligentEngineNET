#include <Api.h>
#include <WindowHandle.h>

#ifdef PLATFORM_WIN32
#include <EngineFactoryD3D11.h>
#include <EngineFactoryD3D12.h>
#endif
#include <EngineFactoryVk.h>
#include <EngineFactoryOpenGL.h>

using namespace Diligent;
#define UNSUPPORTED_PLATFORM_MSG() \
	if(DebugMessageCallback) \
		DebugMessageCallback(DEBUG_MESSAGE_SEVERITY_WARNING, "Platform is not supported", __FUNCTION__, __FILE__, __LINE__)

void utils_get_native_window(WindowHandle* window, NativeWindow& native_window)
{
#if PLATFORM_WIN32
	native_window.hWnd = window->window_handle_;
#elif PLATFORM_UNIVERSAL_WINDOWS
	native_window.pCoreWindow = window->window_handle_;
#elif PLATFORM_LINUX
	LinuxWindowHandle* linux_window = static_cast<LinuxWindowHandle*>(window->window_handle_);
	native_window.WindowId = linux_window->window_id_;
	native_window.pDisplay = linux_window->display_;
	native_window.pXCBConnection = linux_window->xcb_connection_;
#elif PLATFORM_MACOS
	native_window.pNSView = window->window_handle_;
#elif PLATFORM_IOS || PLATFORM_TVOS
	native_window.pCALayer = window->window_handle_;
#elif PLATFORM_ANDROID
	native_window.pAWindow = window->window_handle_;
#elif PLATFORM_EMSCRIPTEN
	native_window.pCanvasId = static_cast<const char*>(window->window_handle_);
#endif
}

void utils_get_gl_create_info(EngineOpenGlCreateInfo* create_info, EngineGLCreateInfo& target)
{
	memcpy(&target, create_info, sizeof(EngineCreateInfo));

	utils_get_native_window(create_info->Window, target.Window);
	target.ZeroToOneNDZ = create_info->ZeroToOneNDZ;
	target.PreferredAdapterType = create_info->PreferredAdapterType;
#if PLATFORM_EMSCRIPTEN
	memcpy(&target.WebGLAttribs, &create_info->WebGlAttribs, sizeof(WebGLContextAttribs));
#endif
}

EXPORT void engine_factory_d3d11_create_swapchain_d3d11(void* factory,
	IRenderDevice* device, 
	IDeviceContext* immediate_ctx,
	SwapChainDesc* swap_chain_desc,
	FullScreenModeDesc* full_screen_mode_desc,
	WindowHandle* window,
	ISwapChain** swap_chain_output)
{
	NativeWindow native_window;
	utils_get_native_window(window, native_window);
	
#if PLATFORM_WIN32 || PLATFORM_UNIVERSAL_WINDOWS
	IEngineFactoryD3D11* d3d_factory = static_cast<IEngineFactoryD3D11*>(factory);
	d3d_factory->CreateSwapChainD3D11(
		device,
		immediate_ctx,
		*swap_chain_desc,
		*full_screen_mode_desc,
		native_window,
		swap_chain_output);
#else
	UNSUPPORTED_PLATFORM_MSG();
	return;
#endif
}

EXPORT void engine_factory_d3d12_create_swap_chain_d3d12(void* factory,
	IRenderDevice* device,
	IDeviceContext* immediate_ctx,
	SwapChainDesc* swap_chain_desc,
	FullScreenModeDesc* full_screen_mode_desc,
	WindowHandle* window,
	ISwapChain** swap_chain_output)
{
	NativeWindow native_window;
	utils_get_native_window(window, native_window);

#if PLATFORM_WIN32 || PLATFORM_UNIVERSAL_WINDOWS
	IEngineFactoryD3D12* d3d_factory = static_cast<IEngineFactoryD3D12*>(factory);
	d3d_factory->CreateSwapChainD3D12(
		device,
		immediate_ctx,
		*swap_chain_desc,
		*full_screen_mode_desc,
		native_window,
		swap_chain_output);
#else
	UNSUPPORTED_PLATFORM_MSG();
	return;
#endif
}

EXPORT void engine_factory_vk_create_swap_chain_vk(void* factory,
	IRenderDevice* device,
	IDeviceContext* immediate_ctx,
	SwapChainDesc* swap_chain_desc,
	WindowHandle* window,
	ISwapChain** swap_chain)
{
	NativeWindow native_window;
	utils_get_native_window(window, native_window);

	IEngineFactoryVk* vk_factory = static_cast<IEngineFactoryVk*>(factory);
	vk_factory->CreateSwapChainVk(
		device,
		immediate_ctx,
		*swap_chain_desc,
		native_window,
		swap_chain);
}

EXPORT void engine_factory_open_gl_create_device_and_swap_chain_gl(IEngineFactoryOpenGL* factory,
	EngineOpenGlCreateInfo* create_info,
	IRenderDevice** device,
	IDeviceContext** immediate_context,
	SwapChainDesc* swap_chain_desc,
	ISwapChain** swap_chain)
{
	EngineGLCreateInfo ci;
	utils_get_gl_create_info(create_info, ci);

	factory->CreateDeviceAndSwapChainGL(
		ci,
		device,
		immediate_context,
		*swap_chain_desc,
		swap_chain);
}

EXPORT void engine_factory_open_gl_attach_to_active_glcontext(IEngineFactoryOpenGL* factory,
	EngineOpenGlCreateInfo* create_info,
	IRenderDevice** device,
	IDeviceContext** immediate_context)
{
	EngineGLCreateInfo ci;
	utils_get_gl_create_info(create_info, ci);
	factory->AttachToActiveGLContext(
		ci,
		device,
		immediate_context);
}
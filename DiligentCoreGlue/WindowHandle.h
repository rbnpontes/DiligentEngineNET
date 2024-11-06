/**
 * This is a workaround used by the Code Generator
 * to handle platform specific operations like SwapChain creation.
 * We can obviously compile C# code for each platform
 * but this will require to us generate bindings for each platform
 * this is a lost of time, for this reason we will only generate
 * native bindings per platform but will not change generated
 * code.
 * And this is the main reason of why exists this file
 */

#pragma once
#include <cstdint>
#include <GraphicsTypes.h>

struct LinuxWindowHandle
{
	uint32_t window_id_;
	void* display_;
	void* xcb_connection_;

};
struct WindowHandle
{
	/**
	 * \brief Specify window handle
	 * - On windows must be a valid HWND
	 * - On windows (UWP) must be a valid CoreWindow pointer
	 * - On linux must be a pointer to LinuxWindowHandle
	 * - On MacOS must be a valid NSView pointer
	 * - On Apple TVOS must be a valid CALayer pointer
	 * - On iOS must be a valid CALayer pointer
	 * - On Android must be a valid AWindow pointer
	 * - On Web must be a canvas id string
	 */
	void* window_handle_;
};

enum WEB_GL_POWER_PREFERENCE : uint8_t
{
	DEFAULT = 0,
	LOW_POWER,
	HIGH_PERFORMANCE
};

struct WebGlContextAttribs
{
	Diligent::Bool Alpha;
	Diligent::Bool Antialias;
	Diligent::Bool PremultipliedAlpha;
	Diligent::Bool PreserveDrawingBuffer;
	WEB_GL_POWER_PREFERENCE PowerPreference;
};

struct EngineOpenGlCreateInfo DILIGENT_DERIVE(Diligent::EngineCreateInfo)
	WindowHandle* Window;
	Diligent::Bool ZeroToOneNDZ;
	Diligent::ADAPTER_TYPE PreferredAdapterType;
	WebGlContextAttribs WebGlAttribs;
};
typedef struct EngineOpenGlCreateInfo EngineOpenGlCreateInfo;
#include <DebugOutput.h>
#include "Api.h"

// test only methods
using namespace Diligent;
EXPORT void exec_debug_message_callback(int severity, const char* message, const char* function, const char* file, int line)
{
	DebugMessageCallback(static_cast<DEBUG_MESSAGE_SEVERITY>(severity), message, function, file, line);
}
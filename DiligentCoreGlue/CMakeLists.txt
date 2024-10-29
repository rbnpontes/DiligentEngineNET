cmake_minimum_required (VERSION 3.6)

project(DiligentCoreGlue CXX)

include(FetchContent)
FetchContent_Declare(
    DiligentCore
    GIT_REPOSITORY https://github.com/DiligentGraphics/DiligentCore.git
    SOURCE_DIR ../_deps/DiligentCore
)
FetchContent_MakeAvailable(DiligentCore)

set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)

file(GLOB SOURCE_FILES *.h *.cpp ../code/Native/*.h ../code/Native/*.cpp)
add_library(DiligentCoreGlue SHARED ${SOURCE_FILES})

target_include_directories(DiligentCoreGlue
PRIVATE
    ${diligentcore_SOURCE_DIR}
    ${CMAKE_CURRENT_SOURCE_DIR}
)

target_link_libraries(DiligentCoreGlue
PRIVATE
    Diligent-BuildSettings
    Diligent-Common
    Diligent-HLSL2GLSLConverterLib
    Diligent-GraphicsEngineD3D11-static
    Diligent-GraphicsEngineD3D12-static
    Diligent-GraphicsEngineOpenGL-static
    Diligent-GraphicsEngineVk-static
)

target_compile_definitions(DiligentCoreGlue PRIVATE GUID=INTERFACE_ID)

set_target_properties(DiligentCoreGlue PROPERTIES OUTPUT_NAME "DiligentCore")

copy_required_dlls(DiligentCoreGlue)
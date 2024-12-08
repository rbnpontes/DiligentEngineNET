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

if(LINUX)
    set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -Os -fPIC -ffunction-sections -fdata-sections")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Os -fPIC -ffunction-sections -fdata-sections")
    set(CMAKE_SHARED_LINKER_FLAGS "${CMAKE_SHARED_LINKER_FLAGS} -Wl,--gc-sections -s")
endif()

file(GLOB SOURCE_FILES *.h *.cpp ../code/Native/*.h ../code/Native/*.cpp)
add_library(DiligentCoreGlue SHARED ${SOURCE_FILES})

target_include_directories(DiligentCoreGlue
PRIVATE
    ${diligentcore_SOURCE_DIR}
    ${CMAKE_CURRENT_SOURCE_DIR}
)

set(DILIGENT_LIBS 
    Diligent-BuildSettings
    Diligent-Common
    Diligent-HLSL2GLSLConverterLib
    Diligent-GraphicsEngineOpenGL-static
    Diligent-GraphicsEngineVk-static
)

if(WIN32)
    list(APPEND DILIGENT_LIBS Diligent-GraphicsEngineD3D11-static Diligent-GraphicsEngineD3D12-static)
endif()

target_link_libraries(DiligentCoreGlue
    PRIVATE ${DILIGENT_LIBS}
)

target_compile_definitions(DiligentCoreGlue PRIVATE GUID=INTERFACE_ID)

set_target_properties(DiligentCoreGlue PROPERTIES OUTPUT_NAME "DiligentCore")

if(WIN32)
    copy_required_dlls(DiligentCoreGlue)
endif()
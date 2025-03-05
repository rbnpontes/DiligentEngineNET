cmake_minimum_required (VERSION 3.6)

project(DiligentCoreGlue CXX)

set(CMAKE_CXX_STANDARD 20)

include(FetchContent)
FetchContent_Declare(
    DiligentCore
    # Custom DiligentCore version, required to make some stuffs work.
    GIT_REPOSITORY https://github.com/rbnpontes/DiligentCore.git
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


if(EMSCRIPTEN)
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -s RELOCATABLE=1 -sEXPORT_ALL=1")
    add_library(DiligentCoreGlue STATIC ${SOURCE_FILES})
else()
    add_library(DiligentCoreGlue SHARED ${SOURCE_FILES})
endif()

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
)

if(!EMSCRIPTEN)
    list(APPEND DILIGENT_LIBS Diligent-GraphicsEngineVk-static)
endif()

if(WIN32)
    list(APPEND DILIGENT_LIBS Diligent-GraphicsEngineD3D11-static Diligent-GraphicsEngineD3D12-static)
endif()

target_link_libraries(DiligentCoreGlue
    PRIVATE ${DILIGENT_LIBS}
)

target_compile_definitions(DiligentCoreGlue PRIVATE GUID=INTERFACE_ID)

set_target_properties(DiligentCoreGlue PROPERTIES OUTPUT_NAME "DiligentCore")
if(EMSCRIPTEN)
    target_compile_definitions(DiligentCoreGlue PRIVATE WEB=1)
    set_target_properties(DiligentCoreGlue PROPERTIES
        LINK_FLAGS "--whole-archive"
    )
    # target_compile_options(DiligentCoreGlue PRIVATE
    #     #"-sEXPORT_ALL=1"
    #     "-sEXPORTED_FUNCTIONS=['_diligent_core_api_ext_get_api_version']"
    # )
endif()

if(WIN32)
    copy_required_dlls(DiligentCoreGlue)
endif()
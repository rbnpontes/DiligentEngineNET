cmake_minimum_required (VERSION 3.6)

project(DiligentCoreGlue CXX)

message(STATUS "CMAKE_SYSTEM_NAME: ${CMAKE_SYSTEM_NAME} ")
set(CMAKE_CXX_STANDARD 20)
# include(FetchContent)
# FetchContent_Declare(
#     DiligentCore
#     # Custom DiligentCore version, required to make some stuffs work.
#     GIT_REPOSITORY https://github.com/rbnpontes/DiligentCore.git
#     SOURCE_DIR ../_deps/DiligentCore
# )
# FetchContent_MakeAvailable(DiligentCore)
add_subdirectory(DiligentCore)

set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)
if(LINUX)
    set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -Os -fPIC -ffunction-sections -fdata-sections")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Os -fPIC -ffunction-sections -fdata-sections")
    set(CMAKE_SHARED_LINKER_FLAGS "${CMAKE_SHARED_LINKER_FLAGS} -Wl,--gc-sections -s")
endif()

file(GLOB SOURCE_FILES *.h *.cpp ../code/Native/*.h ../code/Native/*.cpp)


if(EMSCRIPTEN)
    add_executable(DiligentCoreGlue ${SOURCE_FILES})
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

if(EMSCRIPTEN)
    list(APPEND DILIGENT_LIBS Diligent-GraphicsEngineWebGPU-static)
endif()

target_link_libraries(DiligentCoreGlue
    PRIVATE ${DILIGENT_LIBS}
)

target_compile_definitions(DiligentCoreGlue PRIVATE GUID=INTERFACE_ID)

set_target_properties(DiligentCoreGlue PROPERTIES OUTPUT_NAME "DiligentCore")
if(EMSCRIPTEN)
    file(REAL_PATH "../code/Native/exported-functions.json" EXPORTED_FUNCTIONS_PATH)
    set_target_properties(DiligentCoreGlue PROPERTIES OUTPUT_NAME "libDiligentCore")
    target_compile_definitions(DiligentCoreGlue PRIVATE WEB=1)
    target_link_options(
        DiligentCoreGlue
        PRIVATE
        -g
        -sWASM=1
        -sEXPORT_ES6=1
        -sMODULARIZE=1
        -sALLOW_TABLE_GROWTH=1
        -sNO_EXIT_RUNTIME=1
        -sEXPORTED_FUNCTIONS=@${EXPORTED_FUNCTIONS_PATH}
        -sEXPORTED_RUNTIME_METHODS=[UTF8ToString,stringToNewUTF8,addFunction,removeFunction]
    )

    set_target_properties(DiligentCoreGlue PROPERTIES LINK_FLAGS "--pre-js ${CMAKE_SOURCE_DIR}/ModuleExtensions.js")

    configure_file(${CMAKE_SOURCE_DIR}/sandbox.html ${CMAKE_RUNTIME_OUTPUT_DIRECTORY}/index.html COPYONLY)
endif()

if(WIN32)
    copy_required_dlls(DiligentCoreGlue)
endif()
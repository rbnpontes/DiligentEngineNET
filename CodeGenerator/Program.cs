// See https://aka.ms/new-console-template for more information

using CppAst;
using CodeGenerator;

var diligentCorePath = args[0];
var outDir = args[1];
var glueDir = args[2];

var parserOptions = new CppParserOptions();
parserOptions.Defines.AddRange([
    "PLATFORM_WIN32=1",
    "GUID=INTERFACE_ID",
    "DILIGENT_SHARP_GEN=1"
]);
parserOptions.IncludeFolders.AddRange([
    diligentCorePath,
    Path.Combine(diligentCorePath, "Graphics/GraphicsEngine/interface"),
    Path.Combine(diligentCorePath, "Graphics/GraphicsEngineD3D11/interface"),
    Path.Combine(diligentCorePath, "Graphics/GraphicsEngineD3D12/interface"),
    Path.Combine(diligentCorePath, "Graphics/GraphicsEngineVk/interface"),
    Path.Combine(diligentCorePath, "Graphics/GraphicsEngineOpenGL/interface"),
]);

var compilation = CppParser.ParseFiles([
    Path.Combine(glueDir, "WindowHandle.h"),
    Path.Combine(diligentCorePath, "Primitives/interface/DebugOutput.h"),
    Path.Combine(diligentCorePath, "Primitives/interface/FileStream.h"),
    Path.Combine(diligentCorePath, "Graphics/GraphicsEngine/interface/Dearchiver.h"),
    Path.Combine(diligentCorePath, "Graphics/GraphicsEngineD3D11/interface/EngineFactoryD3D11.h"),
    Path.Combine(diligentCorePath, "Graphics/GraphicsEngineD3D12/interface/EngineFactoryD3D12.h"),
    Path.Combine(diligentCorePath, "Graphics/GraphicsEngineVulkan/interface/EngineFactoryVk.h"),
    Path.Combine(diligentCorePath, "Graphics/GraphicsEngineOpenGL/interface/EngineFactoryOpenGL.h"),
], parserOptions);

if (compilation.HasErrors)
    throw new Exception(compilation.Diagnostics.ToString());

ICodeGenerator[] generators = [
    new CppCodeGenerator(diligentCorePath, outDir, compilation),
    new CSharpCodeGenerator(diligentCorePath, outDir, compilation)
];

foreach (var generator in generators)
{
    generator.Setup();
    generator.Build();
}
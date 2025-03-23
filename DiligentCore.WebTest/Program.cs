using System.Runtime.CompilerServices;
using Diligent;
using Version = Diligent.Version;

// Interop.exec_web_test();
var vertexShaderCode = @"
struct PSInput 
{ 
    float4 Pos   : SV_POSITION; 
    float3 Color : COLOR; 
};

void main(in  uint    VertId : SV_VertexID,
          out PSInput PSIn) 
{
    float4 Pos[3];
    Pos[0] = float4(-0.5, -0.5, 0.0, 1.0);
    Pos[1] = float4( 0.0, +0.5, 0.0, 1.0);
    Pos[2] = float4(+0.5, -0.5, 0.0, 1.0);

    float3 Col[3];
    Col[0] = float3(1.0, 0.0, 0.0); // red
    Col[1] = float3(0.0, 1.0, 0.0); // green
    Col[2] = float3(0.0, 0.0, 1.0); // blue

    PSIn.Pos   = Pos[VertId];
    PSIn.Color = Col[VertId];
}";
var pixelShaderCode = @"
struct PSInput 
{ 
    float4 Pos   : SV_POSITION; 
    float3 Color : COLOR; 
};

struct PSOutput
{ 
    float4 Color : SV_TARGET; 
};

void main(in  PSInput  PSIn,
          out PSOutput PSOut)
{
    PSOut.Color = float4(PSIn.Color.rgb, 1.0);
}
";

var factory = DiligentCore.GetEngineFactoryOpenGL();
//var factory = DiligentCore.GetEngineFactoryWebGPU();
if (factory is null)
    throw new NullReferenceException();

var adapters = factory.EnumerateAdapters(new Version(11, 0));
Console.WriteLine($"Found {adapters.Count()} adapters");
foreach (var adapter in adapters)
{
    var vendor = adapter.Vendor;
    var adapterType = adapter.Type;
    Console.WriteLine($"Description: {adapter.Description}");
    Console.WriteLine($"Vendor: {Enum.GetName(typeof(AdapterVendor), vendor)}");
    Console.WriteLine($"Type: {Enum.GetName(typeof(AdapterType), adapterType)}");
}
factory.SetMessageCallback((severity, message, function, file, line) =>
{
    Console.WriteLine($"[{severity}] {message} {function} ({file}:{line})");
});
//
// // var (device, context) = factory.CreateDeviceAndContext(new EngineWebGPUCreateInfo());
// // var swapChain = factory.CreateSwapChain(device, context, new SwapChainDesc(),
// //     WindowHandleFactory.CreateBrowserWindow("#canvas"));
var (device, context, swapChain) = factory.CreateDeviceAndSwapChain(
    new EngineOpenGlCreateInfo() { Window = WindowHandleFactory.CreateBrowserWindow("#canvas") }, 
    new SwapChainDesc()
);

Console.WriteLine($"Created device: {device}");

//
// IPipelineState CreatePipelineState()
// {
//     var createInfo = new GraphicsPipelineStateCreateInfo();
//     createInfo.PSODesc.Name = "Simple triangle PSO";
//     createInfo.PSODesc.PipelineType = PipelineType.Graphics;
//     createInfo.GraphicsPipeline.NumRenderTargets = 1;
//     createInfo.GraphicsPipeline.RTVFormats[0] = swapChain.Desc.ColorBufferFormat;
//     createInfo.GraphicsPipeline.DSVFormat = swapChain.Desc.DepthBufferFormat;
//     createInfo.GraphicsPipeline.PrimitiveTopology = PrimitiveTopology.TriangleList;
//     createInfo.GraphicsPipeline.RasterizerDesc.CullMode = CullMode.None;
//     createInfo.GraphicsPipeline.DepthStencilDesc.DepthEnable = false;
//
//     var shaderCi = new ShaderCreateInfo();
//     shaderCi.SourceLanguage = ShaderSourceLanguage.Hlsl;
//     shaderCi.EntryPoint = "main";
//     shaderCi.Desc.UseCombinedTextureSamplers = true;
//     
//     shaderCi.Desc.ShaderType = ShaderType.Vertex;
//     shaderCi.Desc.Name = "Triangle vertex shader";
//     shaderCi.Source = vertexShaderCode;
//     
//     using var vertexShader = device.CreateShader(shaderCi);
//
//     shaderCi.Desc.ShaderType = ShaderType.Pixel;
//     shaderCi.Desc.Name = "Triangle pixel shader";
//     shaderCi.Source = pixelShaderCode;
//     
//     using var pixelShader = device.CreateShader(shaderCi);
//     
//     createInfo.VS = vertexShader;
//     createInfo.PS = pixelShader;
//     
//     return device.CreateGraphicsPipelineState(createInfo);
// }
//
// var pipeline = CreatePipelineState();
// void Render()
// {
//     context.SetRenderTargets([swapChain.CurrentBackBufferRTV], swapChain.DepthBufferDSV, ResourceStateTransitionMode.Transition);
//     context.ClearRenderTarget(swapChain.CurrentBackBufferRTV, [0, 0, 0, 1.0f], ResourceStateTransitionMode.Transition);
//     context.ClearDepthStencil(swapChain.DepthBufferDSV, ClearDepthStencilFlags.ClearDepthFlag, 1.0f, 0, ResourceStateTransitionMode.Transition);
//     
//     context.SetPipelineState(pipeline);
//     
//     var drawAttrs = new DrawAttribs();
//     drawAttrs.NumVertices = 3;
//     context.Draw(drawAttrs);
//     
//     swapChain.Present();
// }
//
// Interop.SetRenderLoop(Render);

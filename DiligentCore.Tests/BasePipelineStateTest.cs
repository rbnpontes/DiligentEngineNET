namespace Diligent.Tests;

public class BasePipelineStateTest : BaseRenderTest
{
     public IShader CreateVertexShader()
     {
          var shaderCi = new ShaderCreateInfo()
          {
               Desc = new ShaderDesc() { Name = "Vertex Shader", ShaderType = ShaderType.Vertex },
               Source = "cbuffer GlobalConstants" +
                        "{" +
                        "     float4x4 g_transform;" +
                        "};" +
                        "struct VertexInput" +
                        "{" +
                        "     float3 position: ATTRIB0;" +
                        "};" +
                        "struct VertexOutput" +
                        "{" +
                        "     float4 position: SV_POSITION;" +
                        "};" +
                        "VertexOutput main(VertexInput input)" +
                        "{" +
                        "     VertexOutput output;" +
                        "     output.position = mul(float4(input.position, 1.0), g_transform);" +
                        "     return output;" +
                        "}",
               SourceLanguage = ShaderSourceLanguage.Hlsl,
          };
          return Device.CreateShader(shaderCi);
     }

     public IShader CreatePixelShader()
     {
          var shaderCi = new ShaderCreateInfo()
          {
               Desc = new ShaderDesc() { Name = "Pixel Shader", ShaderType = ShaderType.Pixel },
               Source = "cbuffer GlobalConstants" +
                        "{" +
                        "     float4 g_color;" +
                        "};" +
                        "struct PixelInput" +
                        "{" +
                        "     float4 position: SV_POSITION;" +
                        "};" +
                        "float4 main(PixelInput input): SV_TARGET" +
                        "{" +
                        "     return g_color;" +
                        "}",
               SourceLanguage = ShaderSourceLanguage.Hlsl,
          };
          return Device.CreateShader(shaderCi);
     }
     
     public IPipelineState CreateGraphicsPipelineState()
     {
          using var vertexShader = CreateVertexShader();
          using var pixelShader = CreatePixelShader();
          
          var createInfo = new GraphicsPipelineStateCreateInfo()
          {
               PSODesc = new PipelineStateDesc()
               {
                    Name = "Graphics Pipeline",
                    PipelineType = PipelineType.Graphics,
                    ResourceLayout = new PipelineResourceLayoutDesc()
                    {
                         DefaultVariableType = ShaderResourceVariableType.Dynamic
                    }
               },
               VS = vertexShader,
               PS = pixelShader,
               GraphicsPipeline = new GraphicsPipelineDesc()
               {
                    PrimitiveTopology = PrimitiveTopology.TriangleList,
                    NumRenderTargets = 1,
                    RTVFormats = [TextureFormat.Rgba8Unorm],
                    DSVFormat = TextureFormat.D16Unorm,
                    InputLayout = new InputLayoutDesc()
                    {
                         LayoutElements = [
                              new LayoutElement()
                              {
                                   NumComponents = 3,
                                   IsNormalized = false,
                                   Frequency = InputElementFrequency.PerVertex,
                              }
                         ]
                    },
                    RasterizerDesc = new RasterizerStateDesc()
                    {
                         FillMode = FillMode.Solid,
                    },
                    DepthStencilDesc = new DepthStencilStateDesc()
                    {
                         DepthEnable = true
                    }
               },
          };

          return Device.CreateGraphicsPipelineState(createInfo);
     }
}
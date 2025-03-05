using Diligent;

DiligentCore.SetupLibrary();
var factory = DiligentCore.GetEngineFactoryOpenGL();
if (factory is null)
    throw new NullReferenceException();
factory.SetMessageCallback((severity, message, function, file, line) =>
{
    Console.WriteLine($"[{severity}] {message}");
});

var (device, context, swapChain) = factory.CreateDeviceAndSwapChain(
    new EngineOpenGlCreateInfo() { Window = WindowHandleFactory.CreateBrowserWindow("canvas"), NumImmediateContexts = 3 }, new SwapChainDesc());
context.SetRenderTargets([swapChain.CurrentBackBufferRTV], swapChain.DepthBufferDSV,
    ResourceStateTransitionMode.Transition);
context.ClearDepthStencil(swapChain.DepthBufferDSV, ClearDepthStencilFlags.ClearDepthFlag, 1.0f, 0,
    ResourceStateTransitionMode.Transition);
swapChain.Present();
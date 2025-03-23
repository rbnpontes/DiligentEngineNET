import { dotnet } from './_framework/dotnet.js';
import DiligentCoreModule from './libDiligentCore.js';

const diligentModule = await DiligentCoreModule();
var _renderLoop  = ()=> void(0);
globalThis.setRenderLoop = (callback)=> {
    _renderLoop = callback;
};

const renderLoopMechanism = ()=> {
    _renderLoop();
    window.requestAnimationFrame(renderLoopMechanism);
};
renderLoopMechanism();

const { setModuleImports, runMain, ...instance } = await dotnet
    .withDebugging(1)
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

const diligentModuleMemory = [];
setModuleImports("libDiligentCore.js", {
    ...diligentModule,
});
runMain();

window.dotnetInstance = instance;
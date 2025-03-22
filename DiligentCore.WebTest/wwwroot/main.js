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
    memcpy: (src, dst, size)=> {
        if(sizeof === 0)
            return;
    }
});
runMain();

window.dotnetInstance = instance;
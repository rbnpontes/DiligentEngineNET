import { dotnet } from './_framework/dotnet.js';

var _renderLoop  = ()=> void(0);
globalThis.setRenderLoop = (callback)=> {
    _renderLoop = callback;
};

const renderLoopMechanism = ()=> {
    _renderLoop();
    window.requestAnimationFrame(renderLoopMechanism);
};
renderLoopMechanism();

const instance = await dotnet
    .withDebugging(1)
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();
instance.runMain();

window.dotnetInstance = instance;
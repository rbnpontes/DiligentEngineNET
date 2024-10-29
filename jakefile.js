const buildTasks = require('./BuildScripts/BuildTasks');
const generateTasks = require('./BuildScripts/GenerateTasks');
const testTasks = require('./BuildScripts/TestsTask');

namespace('build', ()=>{
    task('codegen', async () => {
        await buildTasks.buildCodeGen();
    });
    task('native', async () => {
        await buildTasks.buildNative();
    });
    task('bindings', async () => {
        await buildTasks.buildBindings();
    });
});

namespace('generate', ()=> {
    task('native', async ()=> {
        await generateTasks.generateNativeProject();
    });
    task('bindings', async ()=> {
        await generateTasks.generateBindings();
    });
});

task('test', async ()=> {
    await testTasks.runTests();
});
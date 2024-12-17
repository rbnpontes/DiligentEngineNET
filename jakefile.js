const buildTasks = require('./BuildScripts/BuildTasks');
const generateTasks = require('./BuildScripts/GenerateTasks');
const testTasks = require('./BuildScripts/TestsTask');
const packTasks = require('./BuildScripts/PackTasks');
const ciTasks = require('./BuildScripts/CiTasks');

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

namespace('ci', ()=> {
    task('code_artifact', async ()=> {
        await ciTasks.generateCodeArtifact();
    });
    task('bin_artifact', async ()=> {
        await ciTasks.generateBinaryArtifact();
    });
    task('win_artifact', async ()=> {
        await ciTasks.generateWindowsArtifact();
    });
    task('linux_artifact', async ()=> {
        await ciTasks.generateLinuxArtifact();
    });
    task('tag', async ()=> {
        await ciTasks.updateLibVersion(process.env.RELEASE_TAG.replace('release/', ''));
    });
});

namespace('pack', ()=> {
    task('create', async ()=> {
        await packTasks.runPack();
    });
    task('publish', async ()=> {
        await packTasks.runPublish();
    });
});
const { execAsync } = require('./ProcessUtils');
const path = require('path');

const g_source_dir = path.resolve(__dirname, '..');
async function buildCodeGen() {
    process.chdir(g_source_dir);
    await execAsync([
        'dotnet', 'build',
        'CodeGenerator/CodeGenerator.csproj',
        '-c', 'Release',
        `--property:SolutionDir=${g_source_dir}`
    ].join(' '));
}

async function buildNative() {
    process.chdir(g_source_dir);
    
    let error = null;
    // due to diligent dependencies, we must build 
    // at least two times to guarantee success
    // if build fails at second time, we must thrown a error
    for(let i = 0; i < 2; ++i) {
        try {
            await execAsync([
                'cmake', '--build', './glue-build', 
                '--config', 'Release'
            ].join(' '));
            error = null;
            break;
        } catch(e) {
            error = e;
        }
    }

    if(error)
        throw error;
}

async function buildBindings() {
    process.chdir(g_source_dir);
    
    const solution_dir_prop = `--property:SolutionDir=${g_source_dir}`;
    await execAsync([
        'dotnet', 'build',
        'DiligentCore/DiligentCore.csproj',
        '-c', 'Release',
        solution_dir_prop
    ].join(' '));

    console.log('-- Building Unit Tests');

    await execAsync([
        'dotnet', 'build',
        '"DiligentCore.Tests/DiligentCore.Tests.csproj"',
        '-c', 'Release',
        solution_dir_prop
    ].join(' '));
}


module.exports = {
    buildCodeGen,
    buildNative,
    buildBindings
};
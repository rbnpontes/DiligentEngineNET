const path = require('path');
const os = require('os');
const fs = require('fs');
const { execAsync } = require('./ProcessUtils');

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
    const build_path = path.join(g_source_dir, 'glue-build', os.platform());
    let error = null;
    // due to diligent dependencies, we must build 
    // at least two times to guarantee success
    // if build fails at second time, we must thrown a error
    for(let i = 0; i < 2; ++i) {
        try {
            await execAsync([
                'cmake', '--build', build_path, 
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

async function buildNativeWeb() {
    process.chdir(g_source_dir);
    const build_path = path.join(g_source_dir, 'glue-build', 'web');

    async function _build(){
        process.chdir(build_path);
        let error = null;
        // due to diligent dependencies, we must build 
        // at least two times to guarantee success
        // if build fails at second time, we must thrown a error
        for(let i = 0; i < 2; ++i) {
            try {
                await execAsync([
                    'ninja',
                ].join(' '));
                error = null;
                break;
            } catch(e) {
                error = e;
            }
        }
        
        process.chdir(g_source_dir);
        if(error)
            throw error;
    }

    function _removePreviousBuildLib(){
        const lib = path.join(build_path, 'DiligentCore.a');
        if(fs.existsSync(lib))
            fs.unlinkSync(lib);
    }

    function _cleanBinDir() {
        const bin_path = path.join(build_path, 'bin');
        if(!fs.existsSync(bin_path))
            return;
        
        fs.readdirSync(bin_path).forEach(file => {
            fs.unlinkSync(path.join(bin_path, file));
        });
    }
    function _copyArchivesToBin() {
        process.chdir(build_path);
        const bin_path = path.join(build_path, 'bin');
        const archives = fs.globSync('**/*.a').filter(x => !x.startsWith(bin_path));
        if(!fs.existsSync(bin_path))
            fs.mkdirSync(bin_path);
    
        archives.forEach(archive => {
            const file_name = path.basename(archive);
            fs.copyFileSync(archive, path.join(bin_path, file_name));
        });
    }

    async function _mergeArchives() {
        const archives_path = path.join(build_path, 'bin');
        process.chdir(archives_path);
        const archives = fs.globSync('*.a');
        
        while(archives.length > 0) {
            const archive = archives.pop();
            
            console.log(`- Running Command: emar x ${archive}`);
            await execAsync([
                'emar', 'x', archive
            ].join(' '));   
        }

        console.log('- Creating Library Archive');
        await execAsync(['emar', 'rc', 'DiligentCore.a', '*.o'].join(' '));

        console.log('- Indexing Library Archive');
        await execAsync(['emar', 's', 'DiligentCore.a'].join(' '));
    }

    function _copyGeneratedLibToBuildPath() {
        const generated_lib_path = path.join(build_path, 'bin', 'DiligentCore.a');
        const target_path = path.join(build_path, 'DiligentCore.a');

        fs.copyFileSync(generated_lib_path, target_path);
    }

    _removePreviousBuildLib();
    await _build();
    _cleanBinDir();
    _copyArchivesToBin();
    await _mergeArchives();
    _copyGeneratedLibToBuildPath();
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

    console.log('-- Building Test Runner');
    await execAsync([
        'dotnet', 'build',
        '"DiligentCore.TestRunner/DiligentCore.TestRunner.csproj"',
        '-c', 'Release',
        solution_dir_prop
    ].join(' '));
}


module.exports = {
    buildCodeGen,
    buildNative,
    buildNativeWeb,
    buildBindings,
};
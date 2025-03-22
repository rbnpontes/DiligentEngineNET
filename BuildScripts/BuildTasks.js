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

    function _cleanPrevBuildFiles() {
        const paths_2_clean = [
            path.join(build_path, 'obj'),
            path.join(build_path, 'bin'),
        ];

        paths_2_clean.forEach(x => {
            if(!fs.existsSync(x))
                return;

            fs.readdirSync(x).forEach(file => {
                fs.unlinkSync(path.join(x, file));
            });
        });
    }
    function _copyArchivesToBin() {
        process.chdir(build_path);

        const ignored_archives = [];
        const bin_path = path.join(build_path, 'bin');
        const archives = fs.globSync('**/*.a')
            .filter(x => !x.startsWith(bin_path))
            .filter(x => !ignored_archives.find(y => path.basename(x).startsWith(y)));
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
        
        // first, we must extract all object files from all archives
        const archives = fs.readdirSync(archives_path).filter(x => x.endsWith('.a'));
        const processed_obj = {};

        while(archives.length > 0) {
            const archive = archives.pop();
            const archive_name = archive.replace('.a', '');

            console.log(`- Running Command: emar x ${archive}`);
            await execAsync([
                'emar', 'x', archive
            ].join(' '));

            // we must rename objects because they can have the same name
            fs.readdirSync(archives_path).filter(x => x.endsWith('.o') && !processed_obj[x]).forEach(obj => {
                const final_obj_name = [archive_name, obj].join('_');
            
                fs.renameSync(path.join(archives_path, obj), path.join(archives_path, final_obj_name));
                processed_obj[final_obj_name] = true;
            });
        }

        // now, we must run emar to generate the final archive
        console.log('- Running Command: emar rcs DiligentCore.a *.o');

        await execAsync(['emar', 'rcs', 'DiligentCore.a', '*.o'].join(' '));
    }

    function _copyGeneratedLibToBuildPath() {
        const generated_lib_path = path.join(build_path, 'bin', 'DiligentCore.a');
        const target_path = path.join(build_path, 'DiligentCore.a');

        if(fs.existsSync(target_path))
            fs.unlinkSync(target_path);

        fs.copyFileSync(generated_lib_path, target_path);
        fs.unlinkSync(generated_lib_path);
    }

    //_removePreviousBuildLib();
    await _build();
    // _cleanPrevBuildFiles();
    // _copyArchivesToBin();
    // await _mergeArchives();
    // _copyGeneratedLibToBuildPath();
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
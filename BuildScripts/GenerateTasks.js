const { execAsync } = require('./ProcessUtils');
const path = require('path');

const g_source_dir = path.resolve(__dirname, '..');
async function generateBindings() {
    const codegen_path = path.join(g_source_dir, 'net-build', 'CodeGenerator', 'net8.0', 'win-x64');
    const diligent_core_dir = path.join(g_source_dir, '_deps', 'DiligentCore');
    const code_out_dir = path.join(g_source_dir, 'code');

    process.chdir(codegen_path);
    await execAsync([
        'CodeGenerator.exe',
        diligent_core_dir,
        code_out_dir
    ].join(' '));
}
async function generateNativeProject() {
    const glue_path = path.join(g_source_dir, 'DiligentCoreGlue');

    process.chdir(glue_path);
    await execAsync([
        'cmake',
        '-S', '.',
        '-B', '../glue-build'
    ].join(' '));
}

module.exports = { generateBindings, generateNativeProject };
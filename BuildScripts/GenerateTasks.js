const { execAsync } = require('./ProcessUtils');
const path = require('path');
const os = require('os');
const fs = require('fs');

const g_source_dir = path.resolve(__dirname, '..');
async function generateBindings() {
    const codegen_path = path.join(g_source_dir, 'net-build', 'CodeGenerator', 'net8.0', 'win-x64');
    const diligent_core_dir = path.join(g_source_dir, 'glue-build', '_deps', 'DiligentCore');
    const code_out_dir = path.join(g_source_dir, 'code');
    const glue_dir = path.join(g_source_dir, "DiligentCoreGlue");

    process.chdir(codegen_path);
    await execAsync([
        'CodeGenerator.exe',
        diligent_core_dir,
        code_out_dir,
        glue_dir
    ].join(' '));
}
async function generateNativeProject() {
    const glue_path = path.join(g_source_dir, 'DiligentCoreGlue');
    const build_path = path.join(g_source_dir, 'glue-build', os.platform());

    process.chdir(glue_path);
    await execAsync([
        'cmake',
        '-S', '.',
        '-B', build_path
    ].join(' '));
}

module.exports = { generateBindings, generateNativeProject };
const path = require('path');
const fs = require('fs');
const { execAsync } = require('./ProcessUtils');

const g_source_dir = path.resolve(__dirname, '..');
async function runPack() {
    process.chdir(g_source_dir);

    console.log('-- Packing DiligentCore')
    await execAsync([
        'dotnet', 'pack',
        'DiligentCore/DiligentCore.csproj',
        '-c', 'Release',
        `--property:SolutionDir=${g_source_dir}`
    ].join(' '));
}

async function runPublish(params) {
    const output_dir = path.join(g_source_dir, 'net-build/Lib');
    process.chdir(output_dir);

    const packFile = fs.readdirSync(output_dir).find(x => x.endsWith('.nupkg'));

    console.log('-- Publish DiligentCore');
    await execAsync([
        'dotnet', 'nuget',
        'push', packFile,
        '--api-key', process.env.NUGET_API_KEY,
        '--source', 'https://api.nuget.org/v3/index.json'
    ].join(' '));

    process.chdir(g_source_dir);
}

module.exports = { runPack, runPublish };
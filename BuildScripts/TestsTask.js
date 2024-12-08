const { execAsync } = require('./ProcessUtils');
const path = require('path');
const os = require('os');

const g_source_dir = path.resolve(__dirname, '..');

async function runTests() {
    const test_dir = path.join(g_source_dir, 'net-build', 'TestRunner', 'net8.0');

    process.chdir(test_dir);
    const exec_cmd = os.platform() == 'win32' 
        ? ['DiligentCore.TestRunner.exe']
        : ['dotnet', 'DiligentCore.TestRunner.dll'];
    await execAsync(exec_cmd.join(' '));

    process.chdir(g_source_dir);
}

module.exports = { runTests };
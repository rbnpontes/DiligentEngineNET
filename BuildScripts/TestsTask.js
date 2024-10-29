const { execAsync } = require('./ProcessUtils');
const path = require('path');

const g_source_dir = path.resolve(__dirname, '..');

async function runTests() {
    const test_dir = path.join(g_source_dir, 'net-build', 'Tests', 'net8.0');

    process.chdir(test_dir);
    await execAsync([
        'DiligentCore.Tests.exe'
    ].join(' '));
}

module.exports = { runTests };
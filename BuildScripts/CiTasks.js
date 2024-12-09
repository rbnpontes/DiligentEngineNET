const fs = require('fs');
const path = require('path');
const os = require('os');
const archiver = require('archiver');

const g_source_dir = path.resolve(__dirname, '..');
async function generateCodeArtifact() {
    const artifact = fs.createWriteStream(path.join(g_source_dir, 'code-artifact.zip'));
    const archive = archiver('zip', {
        zlib: { level: 9 }
    });
    archive.pipe(artifact);
    archive.directory(path.join(g_source_dir, 'code'), 'code');
    await archive.finalize();
}

async function generateWindowsArtifact() {
    const artifact = fs.createWriteStream(path.join(g_source_dir, 'windows-lib-artifact.zip'));
    const archive = archiver('zip', {
        zlib: { level: 9 }
    });
    archive.pipe(artifact);
    archive.directory(path.join(g_source_dir, 'glue-build/win32/bin'), 'bin');
    await archive.finalize();
}

async function generateLinuxArtifact() {
    const artifact = fs.createWriteStream(path.join(g_source_dir, 'linux-lib-artifact.zip'));
    const archive = archiver('zip', {
        zlib: { level: 9 }
    });
    archive.pipe(artifact);
    archive.file(path.join(g_source_dir, 'glue-build/linux/libDiligentCore.so'), { name: 'libDiligentCore.so' });
    await archive.finalize();
}

async function generateBinaryArtifact() {
    const platform = os.platform();
    if (platform == 'win32')
        await generateWindowsArtifact();
    else if (platform == 'linux')
        await generateLinuxArtifact();
    else
        throw new Error('Not implemented platform');
}

async function updateLibVersion(lib_version) {
    console.log('- Updating Library Version');
    const csproj_path = path.join(g_source_dir, 'DiligentCore', 'DiligentCore.csproj');
    const file = fs.readFileSync(csproj_path).toString()
        .replace('<Version>1.0.0</Version>', `<Version>${lib_version}</Version>`);
    fs.writeFileSync(csproj_path, file);
}

module.exports = {
    generateCodeArtifact,
    generateWindowsArtifact,
    generateLinuxArtifact,
    generateBinaryArtifact,
    updateLibVersion
};
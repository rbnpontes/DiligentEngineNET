const fs = require('fs');
const path = require('path');
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
    const artifact = fs.createWriteStream(path.join(g_source_dir, 'win-artifact.zip'));
    const archive = archiver('zip', {
        zlib: { level: 9 }
    });
    archive.pipe(artifact);
    archive.directory(path.join(g_source_dir, 'glue-build/win32/bin'), 'bin');
    await archive.finalize();
}

async function generateLinuxArtifact() {
    const artifact = fs.createWriteStream(path.join(g_source_dir, 'linux-artifact.zip'));
    const archive = archiver('zip', {
        zlib: { level: 9 }
    });
    archive.pipe(artifact);
    archive.file(path.join(g_source_dir, 'glue-build/linux/libDiligentCore.so'), { name: 'libDiligentCore.so' });
    await archive.finalize();
}

module.exports = { generateCodeArtifact, generateWindowsArtifact, generateLinuxArtifact };
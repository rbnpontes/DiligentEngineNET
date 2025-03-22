const fs = require('fs');
const path = require('path');

const g_source_dir = path.resolve(__dirname, '..');

async function clearWebBinaries(params) {
    const build_path = path.join(g_source_dir, 'glue-build', 'web');
    process.chdir(build_path);

    const files = fs.globSync(['**/*.o', '**/*.a']);
    files.forEach(file => {
        fs.unlinkSync(file);
    });    
}

module.exports = { clearWebBinaries };
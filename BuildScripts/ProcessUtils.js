function execAsync(cmds) {
    return new Promise((resolve)=> {
        jake.exec(cmds, ()=> {
            resolve();
        }, { printStdout : true, printStderr : true });
    });
}

module.exports = { execAsync };
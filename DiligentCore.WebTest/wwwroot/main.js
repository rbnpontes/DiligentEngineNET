import { dotnet } from './_framework/dotnet.js';

await dotnet
    .withDebugging(1)
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();
await dotnet.run();
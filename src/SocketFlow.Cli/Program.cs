using System;
using System.Collections.Generic;
using Folleach.ConsoleUtils;
using SocketFlow.Cli;

T FailedArgs<T>(IEnumerable<string> message) where T : class
{
    Console.WriteLine(message);
    return null;
}

var pargs = new Args(args);

var moduleName = pargs.GetString("m") ?? pargs.GetString("--module") ?? FailedArgs<string>(Messages.ModuleNameNotFound(ArraySegment<string>.Empty));
if (moduleName == null)
    return 1;

return 0;

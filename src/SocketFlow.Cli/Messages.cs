using System.Collections.Generic;

namespace SocketFlow.Cli;

public class Messages
{
    private static readonly string BinaryName = "SocketFlow.Cli.exe";

    public static IEnumerable<string> ModuleNameNotFound(IEnumerable<string> modules)
    {
        yield return $"Select module: '{BinaryName} --module <name>";
        yield return "Possible modules:";
        foreach (var module in modules)
            yield return $" - {module}";
    }
}

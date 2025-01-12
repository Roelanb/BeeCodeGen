using Spectre.Console;

namespace CodeGen.Generators;

public interface IFileGenerator
{
    void IncrementProgress(ProgressTask taskProgress);

    void Generate(string projectPath, string appName);
}

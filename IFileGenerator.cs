namespace CodeGen;

public interface IFileGenerator
{
    void IncrementProgress(ProgressTask progress);
    void Generate(string projectPath, string appName);
}


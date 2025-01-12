using Npgsql;

namespace CodeGen.Generators;

public class GlobalUsingsFileGenerator : BaseGenerator, IFileGenerator
{
    public GlobalUsingsFileGenerator(NpgsqlConnection connection) : base(connection)
    {
    }
    public void IncrementProgress(ProgressTask progress)
    {
        progress.Increment(5);
    }

    public void Generate(string projectPath, string appName)
    {
        var globalUsings = $@"{GenerateFileHeader()}global using FastEndpoints;
global using FastEndpoints.Swagger;
global using Microsoft.AspNetCore.Authorization;
global using Npgsql;";

        File.WriteAllText(Path.Combine(projectPath, "GlobalUsings.cs"), globalUsings);
    }
}



namespace CodeGen.WebGenerators;

public class TsConfigGenerator : BaseGenerator, IFileGenerator
{
    public TsConfigGenerator(NpgsqlConnection connection) : base(connection)
    {
    }
    public void IncrementProgress(ProgressTask progress)
    {
        progress.Increment(5);
    }

    public void Generate(string projectPath, string appName)
    {
        var file = GenerateFileHeader() +
      @"{
  ""files"": [],
  ""references"": [
    { ""path"": ""./tsconfig.app.json"" },
    { ""path"": ""./tsconfig.node.json"" }
  ]
}".Trim();

        File.WriteAllText(Path.Combine(projectPath, "tsconfig.json"), file);
    }
}
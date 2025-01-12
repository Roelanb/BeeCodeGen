using Npgsql;

namespace CodeGen.Generators;

public class LaunchSettingsFileGenerator : BaseGenerator, IFileGenerator
{
    public LaunchSettingsFileGenerator(NpgsqlConnection connection) : base(connection)
    {
    }
    public void IncrementProgress(ProgressTask progress)
    {
        progress.Increment(5);
    }

    public void Generate(string projectPath, string appName)
    {
        Directory.CreateDirectory(Path.Combine(projectPath, "Properties"));

        var launchSettings = $@"{{
  ""$schema"": ""http://json.schemastore.org/launchsettings.json"",
  ""profiles"": {{
    ""{appName}"": {{
      ""commandName"": ""Project"",
      ""dotnetRunMessages"": true,
      ""launchBrowser"": true,
      ""applicationUrl"": ""http://localhost:5001"",
      ""environmentVariables"": {{
        ""ASPNETCORE_ENVIRONMENT"": ""Development""
      }}
    }}
  }}
}}";

        File.WriteAllText(Path.Combine(projectPath, "Properties", "launchSettings.json"), launchSettings);
    }
}

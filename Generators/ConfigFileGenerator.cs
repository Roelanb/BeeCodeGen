
namespace CodeGen.Generators
{
    public class ConfigFileGenerator : IFileGenerator
    {
        private string _connectionString;

        public ConfigFileGenerator(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void Generate(string projectPath, string appName)
        {
            // Create appsettings.json
            var appSettings = $@"{{
  ""Logging"": {{
    ""LogLevel"": {{
      ""Default"": ""Information"",
      ""Microsoft.AspNetCore"": ""Warning""
    }}
  }},
  ""AllowedHosts"": ""*"",
  ""ConnectionStrings"": {{
    ""DefaultConnection"": ""{_connectionString.Replace(" = ", "=")}""
  }}
}}";
            File.WriteAllText(Path.Combine(projectPath, "appsettings.json"), appSettings);

            // Create appsettings.Development.json
            var devSettings = @"
{
  ""Logging"": {
    ""LogLevel"": {
      ""Default"": ""Debug"",
      ""Microsoft.AspNetCore"": ""Information""
    }
  }
}";
            File.WriteAllText(Path.Combine(projectPath, "appsettings.Development.json"), devSettings);
        }

        public void IncrementProgress(ProgressTask progress)
        {
            progress.Increment(5);
        }
    }
}

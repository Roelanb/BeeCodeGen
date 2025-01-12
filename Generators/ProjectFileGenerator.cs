using Npgsql;

namespace CodeGen.Generators;

public class ProjectFileGenerator : IFileGenerator
{
    public void IncrementProgress(ProgressTask progress)
    {
        progress.Increment(5);
    }

    public void Generate(string projectPath, string appName)
    {
        var projectFile = $@"<Project Sdk=""Microsoft.NET.Sdk.Web"">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <InvariantGlobalization>true</InvariantGlobalization>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""FastEndpoints"" Version=""5.33.0"" />
    <PackageReference Include=""FastEndpoints.Swagger"" Version=""5.33.0"" />
    <PackageReference Include=""Dapper"" Version=""2.1.24"" />
    <PackageReference Include=""Npgsql"" Version=""9.0.2"" />
    <PackageReference Include=""Microsoft.AspNetCore.OpenApi"" Version=""7.0.2"" />
    <PackageReference Include=""Swashbuckle.AspNetCore"" Version=""6.4.0"" />
  </ItemGroup>

</Project>";

        File.WriteAllText(Path.Combine(projectPath, $"{appName}.csproj"), projectFile);

        // Generate launchSettings.json
        var launchSettings = $@"{{\n  ""profiles"": {{\n    ""{appName}"": {{\n      ""commandName"": ""Project"",\n      ""launchBrowser"": true,\n      ""environmentVariables"": {{\n        ""ASPNETCORE_ENVIRONMENT"": ""Development""\n      }}\n    }}\n  }}\n}}";

        File.WriteAllText(Path.Combine(projectPath, "Properties", "launchSettings.json"), launchSettings);
    }
}

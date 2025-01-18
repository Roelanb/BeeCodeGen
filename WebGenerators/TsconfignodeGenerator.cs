

namespace CodeGen.WebGenerators;

public class TsconfignodeGenerator : BaseGenerator, IFileGenerator
{
    public TsconfignodeGenerator(NpgsqlConnection connection) : base(connection)
    {
    }
    public void IncrementProgress(ProgressTask progress)
    {
        progress.Increment(5);
    }

    public void Generate(string projectPath, string appName)
    {
        var file = @"{
  ""compilerOptions"": {
    ""target"": ""ES2022"",
    ""lib"": [""ES2023""],
    ""module"": ""ESNext"",
    ""skipLibCheck"": true,
    /* Bundler mode */
    ""moduleResolution"": ""bundler"",
    ""allowImportingTsExtensions"": true,
    ""isolatedModules"": true,
    ""moduleDetection"": ""force"",
    ""noEmit"": true,
    /* Linting */
    ""strict"": true,
    ""noUnusedLocals"": true,
    ""noUnusedParameters"": true,
    ""noFallthroughCasesInSwitch"": true,
    ""noUncheckedSideEffectImports"": true
  },
  ""include"": [""vite.config.ts""]
}
".Trim();

        File.WriteAllText(Path.Combine(projectPath, "tsconfig.node.json"), file);
    }
}
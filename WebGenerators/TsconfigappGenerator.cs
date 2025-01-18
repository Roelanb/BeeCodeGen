using Npgsql;

namespace CodeGen.WebGenerators;

public class TsconfigappGenerator : BaseGenerator, IFileGenerator
{
    public TsconfigappGenerator(NpgsqlConnection connection) : base(connection)
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
  ""compilerOptions"": {
    ""tsBuildInfoFile"": ""./node_modules/.tmp/tsconfig.app.tsbuildinfo"",
    ""target"": ""ES2020"",
    ""useDefineForClassFields"": true,
    ""lib"": [""ES2020"", ""DOM"", ""DOM.Iterable""],
    ""module"": ""ESNext"",
    ""skipLibCheck"": true,
    /* Bundler mode */
    ""moduleResolution"": ""bundler"",
    ""allowImportingTsExtensions"": true,
    ""isolatedModules"": true,
    ""moduleDetection"": ""force"",
    ""noEmit"": true,
    ""jsx"": ""react-jsx"",
    /* Linting */
    ""strict"": true,
    ""noUnusedLocals"": true,
    ""noUnusedParameters"": true,
    ""noFallthroughCasesInSwitch"": true,
    ""noUncheckedSideEffectImports"": true
  },
  ""include"": [""src""]
}".Trim();

        File.WriteAllText(Path.Combine(projectPath, "tsconfig.app.json"), file);
    }
}

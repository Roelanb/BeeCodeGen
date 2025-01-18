namespace CodeGen.WebGenerators;

public class MainTsxGenerator : BaseGenerator, IFileGenerator
{
    public MainTsxGenerator(NpgsqlConnection connection) : base(connection)
    {
    }

    public void IncrementProgress(ProgressTask progress)
    {
        progress.Increment(5);
    }

    public void Generate(string projectPath, string appName)
    {
        var file = GenerateFileHeader() + @"
        import React from ""react"";
import ReactDOM from ""react-dom/client"";
import AppRouter from ""./Router"";
import ""./index.css"";

const rootElement = document.getElementById(""root"");
if (!rootElement) throw new Error(""Failed to find the root element"");
ReactDOM.createRoot(rootElement).render(
  <React.StrictMode>
    <AppRouter />
  </React.StrictMode>
);".Trim();

        var filePath = Path.Combine(projectPath, "src", "main.tsx");
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        File.WriteAllText(filePath, file);
    }
}

namespace CodeGen.WebGenerators;

public class AppCssGenerator : BaseGenerator, IFileGenerator
{
    public AppCssGenerator(NpgsqlConnection connection) : base(connection)
    {
    }

    public void IncrementProgress(ProgressTask progress)
    {
        progress.Increment(5);
    }

    public void Generate(string projectPath, string appName)
    {
        var file = @"
/* App-wide styles */
body {
  margin: 0;
  padding: 0;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen',
    'Ubuntu', 'Cantarell', 'Fira Sans', 'Droid Sans', 'Helvetica Neue',
    sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

code {
  font-family: source-code-pro, Menlo, Monaco, Consolas, 'Courier New',
    monospace;
}

/* Layout styles */
.app-container {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

/* Content area styles */
.content {
  flex: 1;
  padding: 20px;
}".Trim();

        var filePath = Path.Combine(projectPath, "src", "App.css");
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        File.WriteAllText(filePath, file);
    }
}

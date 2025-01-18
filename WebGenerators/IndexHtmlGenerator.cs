

namespace CodeGen.WebGenerators;

public class IndexHtmlGenerator : BaseGenerator, IFileGenerator
{
    public IndexHtmlGenerator(NpgsqlConnection connection) : base(connection)
    {
    }
    public void IncrementProgress(ProgressTask progress)
    {
        progress.Increment(5);
    }

    public void Generate(string projectPath, string appName)
    {
        var file =
      $@"<!doctype html>
<html lang=""en"">
  <head>
    <meta charset = ""UTF-8""/>
    <link rel = ""icon"" type = ""image/svg+xml"" href = ""/vite.svg"" />
    <meta name = ""viewport"" content = ""width=device-width, initial-scale=1.0"" />
    <title > {appName}  </title>
  </head>
  <body>
    <div id = ""root"" ></div>
    <script type = ""module"" src = ""/src/main.tsx"" ></script>
  </body>
</html>
".Trim();

        File.WriteAllText(Path.Combine(projectPath, "index.html"), file);
    }
}
namespace CodeGen.WebGenerators;

public class IndexCssGenerator : BaseGenerator, IFileGenerator
{
    public IndexCssGenerator(NpgsqlConnection connection) : base(connection)
    {
    }

    public void IncrementProgress(ProgressTask progress)
    {
        progress.Increment(5);
    }

    public void Generate(string projectPath, string appName)
    {
        var file = @"/* Font Family Import */
@import url(""https://fonts.googleapis.com/css2?family=Source+Code+Pro:ital,wght@0,200..900;1,200..900&family=Source+Sans+3:ital,wght@0,200..900;1,200..900&display=swap"");

/* Global Style */
html,
body,
#root {
  height: 100%;
  width: 100%;
  font-family: ""Source Sans 3"", sans-serif;
}

::-webkit-scrollbar {
  width: 10px;
}

/* Track */
::-webkit-scrollbar-track {
  background: #e0e0e0;
}

/* Handle */
::-webkit-scrollbar-thumb {
  background: #888;
}

/* Handle on hover */
::-webkit-scrollbar-track:hover {
  background: #555;
}".Trim();

        var filePath = Path.Combine(projectPath, "src", "index.css");
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        File.WriteAllText(filePath, file);
    }
}

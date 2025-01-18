namespace CodeGen.WebGenerators;

public class RouterTsxGenerator : BaseGenerator, IFileGenerator
{
    public RouterTsxGenerator(NpgsqlConnection connection) : base(connection)
    {
    }

    public void IncrementProgress(ProgressTask progress)
    {
        progress.Increment(5);
    }

    public void Generate(string projectPath, string appName)
    {
        var file = GenerateFileHeader() + @"
import { BrowserRouter as Router, Route, Routes } from ""react-router-dom"";
import App from ""./App"";
import Testboard from ""./scenes/testboard"";
import AsfApplication from ""./scenes/asf.application"";
import AsfAction from ""./scenes/asf.action"";

const AppRouter = () => {
  return (
    <Router>
      <Routes>
        <Route path='/' element={<App />}>
          <Route path='/testboard' element={<Testboard />} />
          <Route path='/asf-application' element={<AsfApplication />} />
          <Route path='/asf-action' element={<AsfAction />} />
        </Route>
      </Routes>
    </Router>
  );
};

export default AppRouter;".Trim();

        var filePath = Path.Combine(projectPath, "src", "Router.tsx");
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        File.WriteAllText(filePath, file);
    }
}

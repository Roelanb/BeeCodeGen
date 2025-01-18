namespace CodeGen.WebGenerators;

public class AppTsxGenerator : BaseGenerator, IFileGenerator
{
    public AppTsxGenerator(NpgsqlConnection connection) : base(connection)
    {
    }

    public void IncrementProgress(ProgressTask progress)
    {
        progress.Increment(5);
    }

    public void Generate(string projectPath, string appName)
    {
        var file = GenerateFileHeader() + @"
import { createContext, useState } from ""react"";
import { Box, CssBaseline, ThemeProvider } from ""@mui/material"";
import { ColorModeContext, useMode } from ""./theme"";
import { Outlet } from ""react-router-dom"";
import SideBar from ""./scenes/layout/sidebar"";
import Navbar from ""./scenes/layout/navbar"";

interface ToggledContextType {
  toggled: boolean;
  setToggled: (toggled: boolean) => void;
}

export const ToggledContext = createContext<ToggledContextType | null>(null);

function App() {
  const [theme, colorMode] = useMode();
  const [toggled, setToggled] = useState(false);
  const values = { toggled, setToggled };

  return (
    <ColorModeContext.Provider value={colorMode}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <ToggledContext.Provider value={values}>
          <Box sx={{ display: ""flex"", height: ""100vh"", maxWidth: ""100%"" }}>
            <SideBar />
            <Box
              sx={{
                flexGrow: 1,
                display: ""flex"",
                flexDirection: ""column"",
                height: ""100%"",
                maxWidth: ""100%"",
              }}
            >
              <Navbar />
              <Box sx={{ overflowY: ""auto"", flex: 1, maxWidth: ""100%"" }}>
                <Outlet />
              </Box>
            </Box>
          </Box>
        </ToggledContext.Provider>
      </ThemeProvider>
    </ColorModeContext.Provider>
  );
}

export default App;".Trim();

        var filePath = Path.Combine(projectPath, "src", "App.tsx");
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        File.WriteAllText(filePath, file);
    }
}

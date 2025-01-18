

namespace CodeGen.WebGenerators;

public class PackageJsonGenerator : BaseGenerator, IFileGenerator
{
    public PackageJsonGenerator(NpgsqlConnection connection) : base(connection)
    {
    }
    public void IncrementProgress(ProgressTask progress)
    {
        progress.Increment(5);
    }

    public void Generate(string projectPath, string appName)
    {
        var file =
      $@"{{
           ""name"": ""{appName}"",
            ""private"": true,
            ""version"": ""0.0.0"",
            ""type"": ""module"",
            ""scripts"": {{
                ""dev"": ""vite"",
                ""build"": ""tsc && vite build"",
                ""lint"": ""eslint . --ext ts,tsx --report-unused-disable-directives --max-warnings 0"",
                ""preview"": ""vite preview"",
                ""typecheck"": ""tsc --noEmit""
            }},
            ""dependencies"": {{
                ""react"": ""^18.2.0"",
                ""react-dom"": ""^18.2.0"",
                ""@emotion/react"": ""^11.11.3"",
                ""@emotion/styled"": ""^11.11.0"",
                ""@fullcalendar/core"": ""^6.1.11"",
                ""@fullcalendar/daygrid"": ""^6.1.11"",
                ""@fullcalendar/interaction"": ""^6.1.11"",
                ""@fullcalendar/list"": ""^6.1.11"",
                ""@fullcalendar/react"": ""^6.1.11"",
                ""@fullcalendar/timegrid"": ""^6.1.11"",
                ""@mui/icons-material"": ""^5.15.10"",
                ""@mui/material"": ""^5.15.10"",
                ""@mui/x-data-grid"": ""^6.19.5"",
                ""@nivo/bar"": ""^0.84.0"",
                ""@nivo/core"": ""^0.84.0"",
                ""@nivo/geo"": ""^0.84.0"",
                ""@nivo/line"": ""^0.84.0"",
                ""@nivo/pie"": ""^0.84.0"",
                ""@nivo/stream"": ""^0.84.0"",
                ""axios"": ""^1.7.9"",
                ""formik"": ""^2.4.5"",
                ""react-pro-sidebar"": ""^1.1.0"",
                ""react-router-dom"": ""^6.22.1"",
                ""yup"": ""^1.3.3""
            }},
            ""devDependencies"": {{
                ""@types/react"": ""^18.2.56"",
                ""@types/react-dom"": ""^18.2.19"",
                ""@typescript-eslint/eslint-plugin"": ""^8.19.1"",
                ""@typescript-eslint/parser"": ""^8.19.1"",
                ""@vitejs/plugin-react"": ""^4.2.1"",
                ""@vitejs/plugin-react-swc"": ""^3.7.2"",
                ""eslint"": ""^8.56.0"",
                ""eslint-plugin-react"": ""^7.33.2"",
                ""eslint-plugin-react-hooks"": ""^4.6.0"",
                ""eslint-plugin-react-refresh"": ""^0.4.5"",
                ""typescript"": ""^5.7.3"",
                ""vite"": ""^5.1.4""

                
            }}
        }}".Trim();

        File.WriteAllText(Path.Combine(projectPath, "package.json"), file);
    }
}
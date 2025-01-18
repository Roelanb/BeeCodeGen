

namespace CodeGen.WebGenerators;

public class ViteConfigGenerator : BaseGenerator, IFileGenerator
{
    public ViteConfigGenerator(NpgsqlConnection connection) : base(connection)
    {
    }
    public void IncrementProgress(ProgressTask progress)
    {
        progress.Increment(5);
    }

    public void Generate(string projectPath, string appName)
    {
        var file = GenerateFileHeader() +
      @"import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
})".Trim();

        File.WriteAllText(Path.Combine(projectPath, "vite.config.ts"), file);
    }
}
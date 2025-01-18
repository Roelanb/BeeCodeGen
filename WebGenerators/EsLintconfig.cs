

namespace CodeGen.WebGenerators;

public class EsLintconfigGenerator : BaseGenerator, IFileGenerator
{
    public EsLintconfigGenerator(NpgsqlConnection connection) : base(connection)
    {
    }
    public void IncrementProgress(ProgressTask progress)
    {
        progress.Increment(5);
    }

    public void Generate(string projectPath, string appName)
    {
        var file = $@"
import js from '@eslint/js'
import globals from 'globals'
import reactHooks from 'eslint-plugin-react-hooks'
import reactRefresh from 'eslint-plugin-react-refresh'
import tseslint from 'typescript-eslint'

export default tseslint.config(
  {{ ignores: ['dist'] }},
  {{
    extends: [js.configs.recommended, ...tseslint.configs.recommended],
    files: ['**/*.{{ts,tsx}}'],
    languageOptions: {{
      ecmaVersion: 2020,
      globals: globals.browser,
    }},
    plugins: {{
      'react-hooks': reactHooks,
      'react-refresh': reactRefresh,
    }},
    rules: {{
      ...reactHooks.configs.recommended.rules,
      'react-refresh/only-export-components': [
        'warn',
        {{ allowConstantExport: true }},
      ],
    }},
  }},
)".Trim();

        File.WriteAllText(Path.Combine(projectPath, "eslint.config.js"), file);
    }
}
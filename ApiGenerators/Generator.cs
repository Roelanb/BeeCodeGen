global using System;
global using System.IO;
global using System.Text;
global using System.Text.RegularExpressions;
global using System.Collections.Generic;
global using System.Linq;
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;
global using Npgsql;
global using Dapper;
using Spectre.Console;
using System.Net.NetworkInformation;

namespace CodeGen.ApiGenerators

{
    public class Generator
    {
        private readonly string _deepSeekApiKey;
        private readonly string _connectionString;
        private readonly string _schema;
        private readonly string _tableName;
        private readonly string _outputPath;
        private readonly NpgsqlConnection _db;
        private readonly List<IFileGenerator> _generators;
        private ProgressTask _taskProgress;

        public Generator(ProgressTask taskProgress, string deepSeekApiKey, string connectionString, string schema, string tableName, string outputPath)
        {
            _taskProgress = taskProgress;
            _deepSeekApiKey = deepSeekApiKey;
            _connectionString = connectionString;
            _schema = schema;
            _tableName = tableName;
            _outputPath = outputPath;
            _db = new NpgsqlConnection(connectionString);

            _generators = new List<IFileGenerator>
            {
                new ProjectFileGenerator(),
                new ProgramFileGenerator(_db),
                new GlobalUsingsFileGenerator(_db),
                new LaunchSettingsFileGenerator(_db),
                new EndpointGenerator(_deepSeekApiKey, _db, _schema, _tableName),
                new ConfigFileGenerator(_connectionString),
                new SqlScriptGenerator(_schema, _tableName, _db),
                new DeepSeekMarkdownGenerator(_deepSeekApiKey, _db, _schema, _tableName),          };
        }

        public void Generate()
        {
            var appName = "PlantApi";
            var projectPath = _outputPath + "/DataApi";

            _taskProgress.Increment(10);
            // Create project structure
            Directory.CreateDirectory(projectPath);

            Directory.CreateDirectory(Path.Combine(projectPath, "Features"));
            Directory.CreateDirectory(Path.Combine(projectPath, "Properties"));
            Directory.CreateDirectory(Path.Combine(projectPath, "Controllers"));
            Directory.CreateDirectory(Path.Combine(projectPath, "Models"));
            Directory.CreateDirectory(Path.Combine(projectPath, "Services"));
            _taskProgress.Increment(20);

            // Generate all files
            foreach (var generator in _generators)
            {
                generator.IncrementProgress(_taskProgress);
                generator.Generate(projectPath, appName);
            }

            _taskProgress.Increment(100);

        }
    }
}

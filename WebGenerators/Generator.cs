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

namespace CodeGen.WebGenerators;

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
            new PackageJsonGenerator(_db),
            new ViteConfigGenerator(_db),
            new TsConfigGenerator(_db),
            new TsconfignodeGenerator(_db),
            new TsconfigappGenerator(_db),
            new IndexHtmlGenerator(_db),
            new EsLintconfigGenerator(_db),
            new MainTsxGenerator(_db),
            new RouterTsxGenerator(_db),
            new AppTsxGenerator(_db),
            new AppCssGenerator(_db),
            new IndexCssGenerator(_db),
            new ThemeTsGenerator(_db),

        };
    }

    public void Generate()
    {
        var appName = "PlantWeb";
        var projectPath = _outputPath + "/DataWeb";

        _taskProgress.Increment(10);
        // Create project structure
        Directory.CreateDirectory(projectPath);

        Directory.CreateDirectory(Path.Combine(projectPath, "src"));
        Directory.CreateDirectory(Path.Combine(projectPath, "src", "assets"));
        Directory.CreateDirectory(Path.Combine(projectPath, "src", "api"));
        Directory.CreateDirectory(Path.Combine(projectPath, "src", "components"));
        Directory.CreateDirectory(Path.Combine(projectPath, "src", "data"));
        Directory.CreateDirectory(Path.Combine(projectPath, "src", "scenes"));
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

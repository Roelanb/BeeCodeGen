using Dapper;
using Npgsql;
using System.IO;
using System.Text;

namespace CodeGen.ApiGenerators;

public class SqlScriptGenerator : IFileGenerator
{
    private readonly string _schema;
    private readonly string _table;
    private readonly NpgsqlConnection _db;

    public SqlScriptGenerator(string schema, string table, NpgsqlConnection db)
    {
        _schema = schema;
        _table = table;
        _db = db;
    }
    public void IncrementProgress(ProgressTask progress)
    {
        progress.Increment(5);
    }
    public void Generate(string projectPath, string appName)
    {
        string databasePath = Path.Combine(projectPath, "Database");
        Directory.CreateDirectory(databasePath);
        string sqlFilePath = Path.Combine(databasePath, $"{BaseGenerator.ToPascalCase(_table)}.sql");
        GenerateSqlScript(sqlFilePath);
    }

    public void GenerateSqlScript(string outputPath)
    {
        var columnDefinitions = _db.Query(
            @"SELECT column_name, data_type, is_nullable
              FROM information_schema.columns
              WHERE table_schema = @Schema AND table_name = @Table",
            new { Schema = _schema, Table = _table });

        var createTableStatement = new StringBuilder($"CREATE TABLE {_schema}.{_table} (\\n");
        foreach (var column in columnDefinitions)
        {
            createTableStatement.Append($"    {column.column_name} {column.data_type}");
            createTableStatement.Append(column.is_nullable == "YES" ? "," : " NOT NULL,");
            createTableStatement.AppendLine();
        }
        createTableStatement.Append(");");

        File.WriteAllText(outputPath, createTableStatement.ToString());
    }
}

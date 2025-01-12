using System.Text.RegularExpressions;
using Dapper;
using Npgsql;
using System.IO;

namespace CodeGen.Generators;

public class EndpointGenerator : BaseGenerator, IFileGenerator
{
    private readonly string _deepSeekApiKey;
    private readonly string _schema;
    private readonly string _table;
    // private readonly NpgsqlConnection _db;
    private readonly Dictionary<string, string> _typeMap = new()
    {
        { "character varying", "string" },
        { "text", "string" },
        { "integer", "int" },
        { "int4", "int" },
        { "int8", "long" },
        { "bigint", "long" },
        { "boolean", "bool" },
        { "bool", "bool" },
        { "timestamp", "DateTimeOffset" },
        { "timestamptz", "DateTimeOffset" },
        { "date", "DateTime" },
        { "numeric", "decimal" },
        { "uuid", "Guid" }
    };

    public EndpointGenerator(string deepSeekApiKey, NpgsqlConnection connection, string schema, string table) : base(connection)
    {
        _deepSeekApiKey = deepSeekApiKey;
        _schema = schema;
        _table = table;
    }

    public void IncrementProgress(ProgressTask progress)
    {
        progress.Increment(5);
    }

    public void Generate(string projectPath, string appName)
    {
        string featurePath = Path.Combine(projectPath, "Features", ToPascalCase(_schema), ToPascalCase(_table));

        // Create feature directories
        Directory.CreateDirectory(featurePath);
        Directory.CreateDirectory(Path.Combine(featurePath, $"Create{ToPascalCase(_table)}"));
        Directory.CreateDirectory(Path.Combine(featurePath, $"Get{ToPascalCase(_table)}"));
        Directory.CreateDirectory(Path.Combine(featurePath, $"Update{ToPascalCase(_table)}"));
        Directory.CreateDirectory(Path.Combine(featurePath, $"Delete{ToPascalCase(_table)}"));
        Directory.CreateDirectory(Path.Combine(featurePath, $"Get{ToPascalCase(_table)}ById"));

        // Create Tests directory
        var testsPath = Path.Combine(featurePath, "Tests");
        Directory.CreateDirectory(testsPath);

        // Generate features using dedicated generators
        new CreateFeatureGenerator(_connection, _schema, _table).Generate(featurePath, appName);
        new GetFeatureGenerator(_connection, _schema, _table).Generate(featurePath, appName);
        new GetByIdFeatureGenerator(_connection, _schema, _table).Generate(featurePath, appName);
        new UpdateFeatureGenerator(_connection, _schema, _table).Generate(featurePath, appName);
        new DeleteFeatureGenerator(_connection, _schema, _table).Generate(featurePath, appName);

        // Generate test.http file
        new HttpTestFileGenerator(_connection, _schema, _table).Generate(testsPath, appName);
    }


}

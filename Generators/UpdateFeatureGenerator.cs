using System.IO;
using Npgsql;

namespace CodeGen.Generators;

public class UpdateFeatureGenerator : BaseGenerator
{
    private readonly string _schema;
    private readonly string _table;

    public UpdateFeatureGenerator(NpgsqlConnection connection, string schema, string table)
        : base(connection)
    {
        _schema = schema;
        _table = table;
    }

    public void Generate(string featurePath, string appName)
    {
        var updatePath = Path.Combine(featurePath, $"Update{BaseGenerator.ToPascalCase(_table)}");
        Directory.CreateDirectory(updatePath);

        GenerateModels(updatePath, appName);
        GenerateDataLogic(updatePath, appName);
        GenerateEndpoint(updatePath, appName);
    }

    private void GenerateModels(string updatePath, string appName)
    {
        var tableDefinition = ExtractTableDefinition(_schema, _table);
        var properties = tableDefinition.Columns!.Select(c =>
            $@"    public {GetCSharpType(c.data_type!)}{(c.is_nullable == "YES" ? "?" : "")} {BaseGenerator.ToPascalCase(c.column_name!)} {{ get; set; }}"
        ).ToList();

        var models = $@"{GenerateFileHeader()}#nullable enable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace {appName}.Features.{_schema}.{_table}.Update{BaseGenerator.ToPascalCase(_table)};

public class Request
{{
{string.Join("\n", properties)}
}}

public class Response
{{
{string.Join("\n", properties)}
}}";

        File.WriteAllText(Path.Combine(updatePath, "Models.cs"), models);
    }

    private void GenerateDataLogic(string updatePath, string appName)
    {
        var tableDefinition = ExtractTableDefinition(_schema, _table);
        var primaryKeyColumn = tableDefinition.PrimaryKey!;

        var setClauses = tableDefinition.Columns!
            .Where(c => c.column_name != primaryKeyColumn.column_name)
            .Select(c => $"{c.column_name!} = @{BaseGenerator.ToPascalCase(c.column_name!)}");

        var returningClauses = tableDefinition.Columns!
            .Select(c => $"{c.column_name!} as {BaseGenerator.ToPascalCase(c.column_name!)}");

        var dataLogic = $@"{GenerateFileHeader()}#nullable enable

using Dapper;
using Npgsql;

namespace {appName}.Features.{_schema}.{_table}.Update{BaseGenerator.ToPascalCase(_table)};

public class DataLogic
{{
    private readonly NpgsqlConnection _db;

    public DataLogic(NpgsqlConnection db)
    {{
        _db = db;
    }}

    public async Task<Response?> Update(Request request)
    {{
        var sql = $@""
            UPDATE {_schema}.{_table}
            SET {string.Join(",\n                ", setClauses)}
            WHERE {primaryKeyColumn.column_name} = @{BaseGenerator.ToPascalCase(primaryKeyColumn.column_name!)}
            RETURNING {string.Join(",\n                      ", returningClauses)}
        "";

        return await _db.QuerySingleOrDefaultAsync<Response>(sql, request);
    }}
}}";

        File.WriteAllText(Path.Combine(updatePath, "DataLogic.cs"), dataLogic);
    }

    private void GenerateEndpoint(string updatePath, string appName)
    {
        var endpoint = $@"{GenerateFileHeader()}#nullable enable

using FastEndpoints;
using Npgsql;

namespace {appName}.Features.{_schema}.{_table}.Update{BaseGenerator.ToPascalCase(_table)};

public class Endpoint : Endpoint<Request, Response>
{{
    private readonly NpgsqlConnection _connection;
    private readonly DataLogic _dataLogic;

    public Endpoint(NpgsqlConnection connection)
    {{
        _connection = connection;
        _dataLogic = new DataLogic(_connection);
    }}

    public override void Configure()
    {{
        Put(""/api/{_schema}/{_table}"");
        AllowAnonymous();
    }}

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {{
        var result = await _dataLogic.Update(req);
        
        if (result == null)
        {{
            await SendNotFoundAsync(ct);
            return;
        }}

        await SendOkAsync(result, ct);
    }}
}}";

        File.WriteAllText(Path.Combine(updatePath, "Endpoint.cs"), endpoint);
    }
}

using System.IO;
using Npgsql;

namespace CodeGen.ApiGenerators;

public class CreateFeatureGenerator : BaseGenerator
{
    private readonly string _schema;
    private readonly string _table;

    public CreateFeatureGenerator(NpgsqlConnection connection, string schema, string table)
        : base(connection)
    {
        _schema = schema;
        _table = table;
    }

    public void IncrementProgress(ProgressTask progress)
    {
        progress.Increment(5);
    }

    public void Generate(string featurePath, string appName)
    {
        var createPath = Path.Combine(featurePath, $"Create{BaseGenerator.ToPascalCase(_table)}");
        Directory.CreateDirectory(createPath);

        GenerateModels(createPath, appName);
        GenerateDataLogic(createPath, appName);
        GenerateEndpoint(createPath, appName);
    }


    private void GenerateModels(string createPath, string appName)
    {
        var tableDefinition = ExtractTableDefinition(_schema, _table);

        var requestProperties = string.Join("\n", tableDefinition.Columns!.Select(column =>
            $"    public {GetCSharpType(column.data_type!)} {ToPascalCase(column.column_name!)} {{ get; set; }} = default!;"));

        var responseProperties = string.Join("\n", tableDefinition.Columns!.Select(column =>
            $"    public {GetCSharpType(column.data_type!)} {ToPascalCase(column.column_name!)} {{ get; set; }} = default!;"));

        var models = $@"{GenerateFileHeader()}#nullable enable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace {appName}.Features.{_schema}.{_table}.Create{BaseGenerator.ToPascalCase(_table)};

public class Request
{{
{requestProperties}
}}

public class Response
{{
{responseProperties}
}}";

        File.WriteAllText(Path.Combine(createPath, "Models.cs"), models);
    }

    private void GenerateDataLogic(string createPath, string appName)
    {
        var tableDefinition = ExtractTableDefinition(_schema, _table);

        var dataLogic = $@"{GenerateFileHeader()}#nullable enable

using Dapper;
using Npgsql;

namespace {appName}.Features.{_schema}.{_table}.Create{BaseGenerator.ToPascalCase(_table)};

public class DataLogic
{{
    private readonly NpgsqlConnection _db;

    public DataLogic(NpgsqlConnection db)
    {{
        _db = db;
    }}

    public async Task<Response> Create(Request request)
    {{
        var insertColumns = ""{string.Join(", ", tableDefinition.Columns!.Select(column => column.column_name!))}"";
        var insertParams = ""{string.Join(", ", tableDefinition.Columns!.Select(column => $"@{ToPascalCase(column.column_name!)}"))}"";
        var returningColumns = ""{string.Join(", ", tableDefinition.Columns!.Select(column =>
            $"{column.column_name} as {ToPascalCase(column.column_name!)}"))}"";

        string sql = $@""
            INSERT INTO {_schema}.{_table} ({{ insertColumns}})
            VALUES({{ insertParams}})
            RETURNING {{ returningColumns}}
        "";

        return await _db.QuerySingleAsync<Response>(sql, request);
    }}
}}";

        File.WriteAllText(Path.Combine(createPath, "DataLogic.cs"), dataLogic);
    }

    private void GenerateEndpoint(string createPath, string appName)
    {
        var endpoint = $@"{GenerateFileHeader()}#nullable enable

using FastEndpoints;
using Npgsql;
using Npgsql.PostgresTypes;

namespace {appName}.Features.{_schema}.{_table}.Create{BaseGenerator.ToPascalCase(_table)};

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
        Post(""/api/{_schema}/{_table}"");
        AllowAnonymous();
    }}

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {{
        try
        {{
            var result = await _dataLogic.Create(req);
            await SendOkAsync(result, ct);
        }}
        catch (PostgresException ex) when (ex.SqlState == ""23505"") // Unique violation
        {{
            await SendAsync(null, 409, ct);
        }}
    }}
}}";

        File.WriteAllText(Path.Combine(createPath, "Endpoint.cs"), endpoint);
    }
}

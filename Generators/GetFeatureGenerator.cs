using System.IO;
using Npgsql;

namespace CodeGen.Generators;

public class GetFeatureGenerator : BaseGenerator
{
    private readonly string _schema;
    private readonly string _table;

    public GetFeatureGenerator(NpgsqlConnection connection, string schema, string table) : base(connection)
    {
        _schema = schema;
        _table = table;
    }

    public void Generate(string featurePath, string appName)
    {
        var getPath = Path.Combine(featurePath, $"Get{BaseGenerator.ToPascalCase(_table)}");
        Directory.CreateDirectory(getPath);

        GenerateModels(getPath, appName);
        GenerateDataLogic(getPath, appName);
        GenerateEndpoint(getPath, appName);
    }

    private void GenerateModels(string getPath, string appName)
    {
        var tableDefinition = ExtractTableDefinition(_schema, _table);
        var properties = tableDefinition.Columns!.Select(column =>
            $@"    public {GetCSharpType(column.data_type!)} {ToPascalCase(column.column_name!)} {{ get; set; }} = default!;"
        );

        var models = $@"{GenerateFileHeader()}#nullable enable

namespace {appName}.Features.{_schema}.{_table}.Get{BaseGenerator.ToPascalCase(_table)};

public class Response
{{
{string.Join("\n", properties)}
}}";

        File.WriteAllText(Path.Combine(getPath, "Models.cs"), models);
    }

    private void GenerateDataLogic(string getPath, string appName)
    {
        var tableDefinition = ExtractTableDefinition(_schema, _table);
        var selectColumns = string.Join(",\n                   ",
            tableDefinition.Columns!.Select(column =>
                $"{column.column_name!} as {ToPascalCase(column.column_name!)}"));

        var dataLogic = $@"{GenerateFileHeader()}#nullable enable

using Dapper;
using Npgsql;

namespace {appName}.Features.{_schema}.{_table}.Get{BaseGenerator.ToPascalCase(_table)};

public class DataLogic
{{
    private readonly NpgsqlConnection _db;

    public DataLogic(NpgsqlConnection db)
    {{
        _db = db;
    }}

    public async Task<List<Response>> GetAll()
    {{
        var sql = $@""
            SELECT {selectColumns}
            FROM {_schema}.{_table}"";

        var result = await _db.QueryAsync<Response>(sql);
        return result.ToList();
    }}
}}";

        File.WriteAllText(Path.Combine(getPath, "DataLogic.cs"), dataLogic);
    }

    private void GenerateEndpoint(string getPath, string appName)
    {
        var endpoint = $@"{GenerateFileHeader()}#nullable enable

using FastEndpoints;
using Npgsql;

namespace {appName}.Features.{_schema}.{_table}.Get{BaseGenerator.ToPascalCase(_table)};

public class Endpoint : EndpointWithoutRequest<List<Response>>
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
        Get(""/api/{_schema}/{_table}"");
        AllowAnonymous();
    }}

    public override async Task HandleAsync(CancellationToken ct)
    {{
        var result = await _dataLogic.GetAll();
        await SendOkAsync(result, ct);
    }}
}}";

        File.WriteAllText(Path.Combine(getPath, "Endpoint.cs"), endpoint);
    }
}

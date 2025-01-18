using System.IO;
using Npgsql;

namespace CodeGen.ApiGenerators;

public class GetByIdFeatureGenerator : BaseGenerator
{
    private readonly string _schema;
    private readonly string _table;

    public GetByIdFeatureGenerator(NpgsqlConnection connection, string schema, string table) : base(connection)
    {
        _schema = schema;
        _table = table;
    }

    public void Generate(string featurePath, string appName)
    {
        var getByIdPath = Path.Combine(featurePath, $"Get{BaseGenerator.ToPascalCase(_table)}ById");
        Directory.CreateDirectory(getByIdPath);

        GenerateModels(getByIdPath, appName);
        GenerateDataLogic(getByIdPath, appName);
        GenerateEndpoint(getByIdPath, appName);
    }

    private void GenerateModels(string getByIdPath, string appName)
    {
        var tableDefinition = ExtractTableDefinition(_schema, _table);
        var properties = tableDefinition.Columns!.Select(c =>
            $@"    public {GetCSharpType(c.data_type!)}{(c.is_nullable == "YES" ? "?" : "")} {BaseGenerator.ToPascalCase(c.column_name!)} {{ get; set; }}"
        ).ToList();

        var models = $@"{GenerateFileHeader()}#nullable enable

namespace {appName}.Features.{_schema}.{_table}.Get{BaseGenerator.ToPascalCase(_table)}ById;

public class Response
{{
{string.Join("\n", properties)}
}}";

        File.WriteAllText(Path.Combine(getByIdPath, "Models.cs"), models);
    }

    private void GenerateDataLogic(string getByIdPath, string appName)
    {
        var tableDefinition = ExtractTableDefinition(_schema, _table);
        var primaryKey = tableDefinition.PrimaryKey!;
        var primaryKeyType = GetCSharpType(primaryKey.data_type!);

        var selectColumns = tableDefinition.Columns!
            .Select(c => $"{c.column_name!} as {BaseGenerator.ToPascalCase(c.column_name!)}");

        var dataLogic = $@"{GenerateFileHeader()}#nullable enable

using Dapper;
using Npgsql;

namespace {appName}.Features.{_schema}.{_table}.Get{BaseGenerator.ToPascalCase(_table)}ById;

public class DataLogic
{{
    private readonly NpgsqlConnection _db;

    public DataLogic(NpgsqlConnection db)
    {{
        _db = db;
    }}

    public async Task<Response?> GetById({primaryKeyType} id)
    {{
        var sql = $@""
            SELECT {string.Join(",\n                   ", selectColumns)}
            FROM {_schema}.{_table}
            WHERE {primaryKey.column_name} = @id"";

        return await _db.QuerySingleOrDefaultAsync<Response>(sql, new {{ id }});
    }}
}}";

        File.WriteAllText(Path.Combine(getByIdPath, "DataLogic.cs"), dataLogic);
    }

    private void GenerateEndpoint(string getByIdPath, string appName)
    {
        var tableDefinition = ExtractTableDefinition(_schema, _table);
        var primaryKey = tableDefinition.PrimaryKey!;
        var primaryKeyType = GetCSharpType(primaryKey.data_type!);

        var endpoint = $@"{GenerateFileHeader()}#nullable enable

using FastEndpoints;
using Npgsql;

namespace {appName}.Features.{_schema}.{_table}.Get{BaseGenerator.ToPascalCase(_table)}ById;

public class Endpoint : EndpointWithoutRequest<Response>
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
        Get(""/api/{_schema}/{_table}/{{id}}"");
        AllowAnonymous();
    }}

    public override async Task HandleAsync(CancellationToken ct)
    {{
        var id = Route<{primaryKeyType}?>(""id"") ?? throw new ArgumentNullException(""id"", ""ID cannot be null"");
        var result = await _dataLogic.GetById(id);
        
        if (result == null)
        {{
            await SendNotFoundAsync(ct);
            return;
        }}

        await SendOkAsync(result, ct);
    }}
}}";

        File.WriteAllText(Path.Combine(getByIdPath, "Endpoint.cs"), endpoint);
    }
}

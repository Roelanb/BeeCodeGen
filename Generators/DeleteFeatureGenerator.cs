using System.IO;
using Npgsql;

namespace CodeGen.Generators;

public class DeleteFeatureGenerator : BaseGenerator
{
    private readonly string _schema;
    private readonly string _table;

    public DeleteFeatureGenerator(NpgsqlConnection connection, string schema, string table) : base(connection)
    {
        _schema = schema;
        _table = table;
    }


    public void Generate(string featurePath, string appName)
    {
        var deletePath = Path.Combine(featurePath, $"Delete{BaseGenerator.ToPascalCase(_table)}");
        Directory.CreateDirectory(deletePath);

        GenerateDataLogic(deletePath, appName);
        GenerateEndpoint(deletePath, appName);
    }

    private void GenerateDataLogic(string deletePath, string appName)
    {
        var tableDefinition = ExtractTableDefinition(_schema, _table);
        var primaryKey = tableDefinition.PrimaryKey!;
        var primaryKeyType = GetCSharpType(primaryKey.data_type!);

        var dataLogic = $@"{GenerateFileHeader()}#nullable enable

using Dapper;
using Npgsql;

namespace {appName}.Features.{_schema}.{_table}.Delete{BaseGenerator.ToPascalCase(_table)};

public class DataLogic
{{
    private readonly NpgsqlConnection _db;

    public DataLogic(NpgsqlConnection db)
    {{
        _db = db;
    }}

    public async Task<bool> Delete({primaryKeyType} id)
    {{
        var checkSql = $@""SELECT 1 FROM {_schema}.{_table}
                    WHERE {primaryKey.column_name} = @id"";

        var exists = await _db.ExecuteScalarAsync<int?>(checkSql, new {{ id }});
        if (exists == null)
        {{
            return false;
        }}

        var deleteSql = $@""DELETE FROM {_schema}.{_table}
                    WHERE {primaryKey.column_name} = @id"";

        await _db.ExecuteAsync(deleteSql, new {{ id }});
        return true;
    }}
}}";

        File.WriteAllText(Path.Combine(deletePath, "DataLogic.cs"), dataLogic);
    }

    private void GenerateEndpoint(string deletePath, string appName)
    {
        var tableDefinition = ExtractTableDefinition(_schema, _table);
        var primaryKey = tableDefinition.PrimaryKey!;
        var primaryKeyType = GetCSharpType(primaryKey.data_type!);

        var endpoint = $@"{GenerateFileHeader()}#nullable enable

using FastEndpoints;
using Npgsql;

namespace {appName}.Features.{_schema}.{_table}.Delete{BaseGenerator.ToPascalCase(_table)};

public class Endpoint : EndpointWithoutRequest
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
        Delete(""/api/{_schema}/{_table}/{{id}}"");
        AllowAnonymous();
    }}

    public override async Task HandleAsync(CancellationToken ct)
    {{
        var id = Route<{primaryKeyType}?>(""id"") ?? throw new ArgumentNullException(""id"", ""ID cannot be null"");
        var deleted = await _dataLogic.Delete(id);
        if (!deleted)
        {{
            await SendNotFoundAsync(ct);
            return;
        }}
        await SendNoContentAsync(ct);
    }}
}}";

        File.WriteAllText(Path.Combine(deletePath, "Endpoint.cs"), endpoint);
    }
}

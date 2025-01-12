using System.Text.Json;
using Npgsql;

namespace CodeGen.Generators;

public class HttpTestFileGenerator : BaseGenerator, IFileGenerator
{
    private readonly string _schema;
    private readonly string _table;

    public HttpTestFileGenerator(NpgsqlConnection connection, string schema, string table) : base(connection)
    {
        _schema = schema;
        _table = table;
    }
    public void IncrementProgress(ProgressTask progress)
    {
        progress.Increment(5);
    }

    public void Generate(string projectPath, string appName)
    {
        var tableDefinition = ExtractTableDefinition(_schema, _table);

        var testData = tableDefinition.Columns!.ToDictionary(c => ToPascalCase(c.column_name!), c => c.TestValue);
        var options = new JsonSerializerOptions { WriteIndented = true };
        var testFile = $@"{GenerateFileHeader()}@baseUrl = http://localhost:5001

### Get all {_table}
GET {{{{baseUrl}}}}/api/{_schema}/{_table}

### Create new {_table}
POST {{{{baseUrl}}}}/api/{_schema}/{_table}
Content-Type: application/json

{JsonSerializer.Serialize(testData, options)}

### Update {_table}
PUT {{{{baseUrl}}}}/api/{_schema}/{_table}
Content-Type: application/json

{JsonSerializer.Serialize(testData, options)}   

### Get {_table} by ID
GET {{{{baseUrl}}}}/api/{_schema}/{_table}/{tableDefinition.ApiPrimaryKeyParameterUrl}

### Delete {_table}
DELETE {{{{baseUrl}}}}/api/{_schema}/{_table}/{tableDefinition.ApiPrimaryKeyParameterUrl}";

        Console.WriteLine($"Test file generated successfully at: {Path.Combine(projectPath, "test.http")}");
        File.WriteAllText(Path.Combine(projectPath, "test.http"), testFile);
    }
}

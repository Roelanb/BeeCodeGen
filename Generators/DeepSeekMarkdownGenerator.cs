using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Npgsql;

namespace CodeGen.Generators;

public class DeepSeekMarkdownGenerator : BaseGenerator, IFileGenerator
{
    private readonly string _apiKey;
    private readonly string _schema;
    private readonly string _table;
    private readonly HttpClient _httpClient;

    public DeepSeekMarkdownGenerator(string deepSeekApiKey, NpgsqlConnection connection, string schema, string table)
        : base(connection)
    {
        _apiKey = deepSeekApiKey;
        _schema = schema;
        _table = table;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.deepseek.com/v1/")
        };
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    public void Generate(string projectPath, string appName)
    {
        string databasePath = Path.Combine(projectPath, "Database");
        Directory.CreateDirectory(databasePath);
        GenerateWithLlmAsync(projectPath).GetAwaiter().GetResult();
    }

    public void IncrementProgress(ProgressTask progress)
    {
        progress.Increment(5);
    }

    public async Task GenerateWithLlmAsync(string featurePath)
    {

        var outputPath = Path.Combine(featurePath, "Database", $"{BaseGenerator.ToPascalCase(_table)}.md");


        var prompt = $"Generate markdown documentation for the SQL table {_schema}.{_table}. " +
            "Include a table with columns: Column Name, Data Type, Is Nullable, Default Value, Description. " +
            "Format the output as a markdown table. Also add the SQL script to create the table.";

        var tableDefinition = ExtractTableDefinition(_schema, _table);

        prompt += $"SQL table definition: {JsonSerializer.Serialize(tableDefinition)}";

        var request = new
        {
            model = "deepseek-chat",
            messages = new[]
            {
                new { role = "user", content = prompt }
            },
            temperature = 0.7,
            max_tokens = 1000
        };

        var response = await _httpClient.PostAsJsonAsync("chat/completions", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<DeepSeekResponse>();
        var markdown = result?.Choices.FirstOrDefault()?.Message.Content;

        if (!string.IsNullOrEmpty(markdown))
        {
            await File.WriteAllTextAsync(outputPath, markdown);
        }
    }

    private class DeepSeekResponse
    {
        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; } = new();

        public class Choice
        {
            [JsonPropertyName("message")]
            public Message Message { get; set; } = new();
        }

        public class Message
        {
            [JsonPropertyName("content")]
            public string Content { get; set; } = string.Empty;
        }
    }
}

using Npgsql;

namespace CodeGen.ApiGenerators;

public class ProgramFileGenerator : BaseGenerator, IFileGenerator
{
    public ProgramFileGenerator(NpgsqlConnection connection) : base(connection)
    {
    }
    public void IncrementProgress(ProgressTask progress)
    {
        progress.Increment(5);
    }

    public void Generate(string projectPath, string appName)
    {
        var program = GenerateFileHeader() + @"
var builder = WebApplication.CreateBuilder(args);

// Add configuration sources
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(""appsettings.json"", optional: false, reloadOnChange: true)
    .AddJsonFile($""appsettings.{builder.Environment.EnvironmentName}.json"", optional: true)
    .AddEnvironmentVariables();

// Log configuration values for debugging
Console.WriteLine(""Configuration values:"");
Console.WriteLine($""DefaultConnection: {builder.Configuration.GetConnectionString(""DefaultConnection"")}"");

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();
builder.Services.AddAuthorization();

// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(""http://localhost:5173"")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add PostgreSQL connection
builder.Services.AddScoped(_ => new NpgsqlConnection(builder.Configuration.GetConnectionString(""DefaultConnection"")));

var app = builder.Build();

app.UseAuthorization();
app.UseCors();
app.UseFastEndpoints();
app.UseOpenApi();
app.UseSwaggerUI();

app.Run();";

        File.WriteAllText(Path.Combine(projectPath, "Program.cs"), program);
    }
}

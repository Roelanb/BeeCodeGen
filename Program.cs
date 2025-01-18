
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using Npgsql;
using Dapper;
using Microsoft.Extensions.Configuration;
using CodeGen.ApiGenerators;
using System.ComponentModel;

namespace CodeGen
{
    enum OutputType
    {
        DataApi = 1,
        WebSite = 2,
        Exit = 3
    }

    class Program
    {
        static void Main(string[] args)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Markup("[bold]Welcome to CodeGen![/]"));
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Markup($"[bold]Version:[/] {typeof(Program).Assembly.GetName().Version}"));
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();


            var outputTable = new Table();
            outputTable.AddColumn("Output");
            outputTable.AddRow("1. Data API");
            outputTable.AddRow("2. Web Site");
            outputTable.AddRow("3. Exit");

            AnsiConsole.Write(outputTable);

            var outputSelection = int.Parse(Console.ReadLine() ?? "1");

            if (outputSelection == 3) return;

            var outputType = OutputType.DataApi;

            if (outputSelection == (int)OutputType.WebSite)
            {
                outputType = OutputType.WebSite;
            }


            // Configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            var configuration = builder.Build();

            // ask the user for the connection string
            // Get connection strings
            var connectionStrings = configuration.GetSection("ConnectionStrings").GetChildren();

            if (connectionStrings.Count() == 0)
            {
                throw new InvalidOperationException("No connection strings found in the appsettings.json or user secrets.");
            }

            AnsiConsole.Write(new Markup("[bold]Available connection strings:[/]"));
            AnsiConsole.WriteLine();

            var connectionStringTable = new Table();
            connectionStringTable.AddColumn("Connection string");

            for (var i = 0; i < connectionStrings.Count(); i++)
            {
                connectionStringTable.AddRow($"{i + 1}. {connectionStrings.ElementAt(i).Key}");
            }

            connectionStringTable.AddRow($"{connectionStrings.Count() + 1}. Exit");

            AnsiConsole.Write(connectionStringTable);

            var connectionStringIndex = int.Parse(Console.ReadLine() ?? "1") - 1;

            if (connectionStringIndex == connectionStrings.Count())
            {
                Console.WriteLine("Exiting...");
                return;
            }

            var selectedConnectionString = connectionStrings.ElementAt(connectionStringIndex).Value;

            var deepSeekApiKey = configuration["DeepSeek:ApiKey"] ??
                throw new InvalidOperationException("DeepSeek API key not found in the appsettings.json or user secrets.");


            using var connection = new NpgsqlConnection(selectedConnectionString);
            connection.Open();

            // Get all schemas
            var schemas = connection.Query<string>("SELECT schema_name FROM information_schema.schemata WHERE schema_name NOT IN ('pg_catalog', 'information_schema')").ToList();

            AnsiConsole.Write(new Markup("[bold]Available schemas:[/]"));
            AnsiConsole.WriteLine();

            var schemaTable = new Table();
            schemaTable.AddColumn("Schema");

            for (int i = 0; i < schemas.Count; i++)
            {
                schemaTable.AddRow($"{i + 1}. {schemas[i]}");
            }
            schemaTable.AddRow($"{schemas.Count + 1}. Exit");
            AnsiConsole.Write(schemaTable);



            var schemaIndex = int.Parse(Console.ReadLine() ?? "1") - 1;

            if (schemaIndex == schemas.Count)
            {
                Console.WriteLine("Exiting...");
                return;
            }

            var selectedSchema = schemas[schemaIndex];

            // Get all tables in selected schema
            var tables = connection.Query<string>(
                "SELECT table_name FROM information_schema.tables WHERE table_schema = @schema AND table_type = 'BASE TABLE'",
                new { schema = selectedSchema }).ToList();


            AnsiConsole.Write(new Markup("[bold]Available Tables:[/]"));
            AnsiConsole.WriteLine();

            var tableTable = new Table();
            tableTable.AddColumn("Table");

            for (int i = 0; i < tables.Count; i++)
            {
                tableTable.AddRow($"{i + 1}. {tables[i]}");
            }
            tableTable.AddRow($"{tables.Count + 1}. All tables");
            tableTable.AddRow($"{tables.Count + 2}. Exit");

            AnsiConsole.Write(tableTable);

            // path where the api will be generated
            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "DataApiGenerated");


            var tableIndex = int.Parse(Console.ReadLine() ?? "1") - 1;



            if (tableIndex == tables.Count + 1)
            {
                Console.WriteLine("Exiting...");
                return;
            }
            else if (tableIndex == tables.Count)
            {


                AnsiConsole.Progress()
                    .Start(ctx =>
                    {
                        var tasks = new Dictionary<string, ProgressTask>();

                        foreach (var table in tables)
                        {
                            tasks.Add(table, ctx.AddTask($"[green]Generating API for {table}[/]"));
                        }

                        while (!ctx.IsFinished)
                        {
                            foreach (var table in tables)
                            {
                                var generator = new ApiGenerators.Generator(tasks[table], deepSeekApiKey, selectedConnectionString!, selectedSchema, table, outputPath);
                                generator.Generate();
                            }
                        }
                    });

            }
            else
            {
                AnsiConsole.Progress()
                    .Start(ctx =>
                    {
                        var selectedTable = tables[tableIndex];
                        var task = ctx.AddTask($"[green]Generating API for {selectedTable}[/]");

                        switch (outputType)
                        {
                            case OutputType.DataApi:
                                var generator = new ApiGenerators.Generator(task, deepSeekApiKey, selectedConnectionString!, selectedSchema, selectedTable, outputPath);
                                generator.Generate();
                                break;
                            case OutputType.WebSite:
                                var generatorWeb = new WebGenerators.Generator(task, deepSeekApiKey, selectedConnectionString!, selectedSchema, selectedTable, outputPath);
                                generatorWeb.Generate();
                                break;

                        }
                    });
            }

        }
    }
}

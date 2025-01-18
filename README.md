# BeeCodeGen

## Project Description

BeeCodeGen is a C# application designed to streamline the process of generating code for various use cases. It provides a configurable environment with support for connection strings and API keys, enabling developers to easily manage their application settings. The project includes a demo configuration file for easy setup and testing, ensuring that sensitive information is kept secure by excluding the main configuration file from version control.

## Features

- Automatic endpoint generation for CRUD operations
- Database schema-driven code generation
- Support for PostgreSQL database
- Generates feature-based directory structure
- Creates HTTP test files for API testing
- Configurable logging levels for different environments
- Type mapping for database to C# types
- Generates both development and production configuration files

## Configuration

### Setting up appsettings.json

1. Create an `appsettings.json` file in the root directory with the following structure:
   (you can find an example in appsettings.demo.json)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "your_connection_string_here"
  },
  "DeepSeekApiKey": "your_api_key_here"
}
```

### Configuration Parameters

- `ConnectionStrings:DefaultConnection`: Your PostgreSQL connection string
- `DeepSeekApiKey`: API key for DeepSeek integration (if required)
- `Logging:LogLevel`: Configure logging levels for different components

## Getting Started

1. Clone the repository
2. Configure your `appsettings.json` and `appsettings.Development.json` files
3. Build and run the application
4. The application will generate code based on your database schema and configuration

## Future Improvements

- Add a full testing approach to test out the generated code
- Add support for other database types
- Add support for other code generation frameworks
- Add support for more advanced features, such as code generation for specific endpoints or custom configurations

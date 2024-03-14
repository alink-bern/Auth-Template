# .NET 8 Login Template


## Table of Contents
- [Installation](#installation)
- [Configuration](#configuration)


## Introduction
My Awesome .NET 8 Web API is designed to provide seamless data access and manipulation. Whether you're building a mobile app, a web frontend, or integrating with other services, this API has got you covered!

## Installation
1. Clone this repository to your local machine.
2. Install the necessary dependencies using `dotnet restore`.
3. Configure your secrets (see [Configuration](#configuration)).
4. Run the API using `dotnet run`.


## Configuration
To configure your secrets with the following information:

```json
{
  "ConnectionStrings": {
    "Development": "Server=server;Database=dev;User id=username;Password=password;TrustServerCertificate=true;",
    "Production": "Server=server;Database=ProdDB;User id=username;Password=password;TrustServerCertificate=true;"
  },
  "TokenSecret": "TokenSecret"
}
```


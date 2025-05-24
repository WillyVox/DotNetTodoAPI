## PROJECT - DOTNET TODO API 


### Install DOTNET for MAC
    1. .NET 8.0 SDK
        https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-8.0.410-macos-x64-installer
    2. In Terminal
        $ dotnet

### Steps to Build the REST API
    1. Create a New Project: Run the following command in your terminal to create a new ASP.NET Core Web API project:
        $ dotnet new webapi -n TodoApi
    2. Add Dependencies: Add Entity Framework Core with the in-memory database provider:
        $ dotnet add package Microsoft.EntityFrameworkCore.InMemory

### Run app
    $ dotnet run

### Test app
    $ http://localhost:5004/weatherforecast (in browser)
    or $ curl http://localhost:5004/weatherforecast 


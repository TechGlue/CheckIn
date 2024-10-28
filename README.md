[![CheckMeInService](https://github.com/TechGlue/CheckIn/actions/workflows/master_checkmeinservice.yml/badge.svg)](https://github.com/TechGlue/CheckIn/actions/workflows/master_checkmeinservice.yml)
# Welcome to the CheckIn Service API 🎉
**Hey there!** Welcome to the CheckIn API! This project serves as a sandbox for sharpening my skills with ASP.NET Core and new language features and technologies while solving a personal problem: tracking and counting consistency in habit-building.

## What's under the hood?
This app is built using: 
- C#
- .NET 8 (latest LTS)
- Test Containers
- Azure SQL
- Azure

## Want to run the API locally? 

### Prerequisites 🛠️
-   [Git](http://git-scm.com/)
-   [Azure account and exposure to Azure](https://portal.azure.com)
-   [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

### Installation Steps :wrench:

**Setting up infrastructure**  
- Launch Azure
- Create a new app registration in EntraId
- Expose an API and create the two scopes **subscribers and checkins**
- Create a new Azure SQL DB instance
- Run **CheckMeIn_InitTables_SP_01.sql** in the new DB instance
- Execute the stored procedure "Exec dbo.InitTables"
- Fill in the EntraId and AzureSQL login fields in the appsettings.json
- Links: support with [EntraId config files](https://learn.microsoft.com/en-us/entra/identity-platform/scenario-web-app-sign-user-app-configuration?tabs=aspnetcore#configuration-files)

Now fire up your terminal and run:
```sh
$ git clone https://github.com/TechGlue/CheckIn.git
$ cd CheckIn
$ cd CheckMeInService
$ dotnet restore 
$ dotnet run
```

Query the database using an API testing tool. I prefer Postman, as it is the most straightforward for fetching the OAuth2 bearer token and authorizing access to the API. [Here's](https://youtu.be/cUcn1gm_f-8?t=899) a tutorial that I found helpful for getting started with Postman and EntraId

**Future work**: building up a development container with a local database. 

## Misc.
[Design and architecture diagrams]()


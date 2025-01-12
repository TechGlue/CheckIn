[![CheckMeInService](https://github.com/TechGlue/CheckIn/actions/workflows/master_checkmeinservice.yml/badge.svg)](https://github.com/TechGlue/CheckIn/actions/workflows/master_checkmeinservice.yml)
# Welcome to the CheckIn Service API 🎉
**Hey there!** Welcome to the CheckIn API! This personal project serves as a sandbox for sharpening my skills with ASP.NET Core and new language features and technologies while solving a personal problem: tracking and counting consistency in habit-building.

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
Now fire up your terminal and run:

```sh
$ git clone https://github.com/TechGlue/CheckIn.git
$ cd CheckIn
$ cd CheckMeInService
```

**Setting up infrastructure**  
- Launch Azure
- Create a new app registration in EntraId
- Expose an API and create the two scopes **subscribers and checkins**
- Create a new Azure SQL DB instance
- Run **CheckMeIn_InitTables_SP_01.sql** in the new DB instance
- Execute the stored procedure "Exec dbo.InitTables"
- Fill in the EntraId and AzureSQL login fields in the appsettings.json
- Support with [EntraId config files](https://learn.microsoft.com/en-us/entra/identity-platform/scenario-web-app-sign-user-app-configuration?tabs=aspnetcore#configuration-files)

**Running the API**
```sh
$ dotnet restore 
$ dotnet run
```

Query the database using an API testing tool. I prefer Postman, as it is the most straightforward for fetching the OAuth2 bearer token and authorizing access to the API. [Here's](https://youtu.be/cUcn1gm_f-8?t=899) a tutorial that I found helpful for getting started with Postman and EntraId

### Routes
**/api/checkins/**
- **unenroll** (Removes user given a phone number/email from the checkins table)<br/>
**Parameters:**
  - phoneNumber
- **logdaily** (Creates a new checkin entry given a phone number/email.)<br/>
**Parameters:**
  - phoneNumber

**/api/subscribers/**
- **addsubscription** (Creates a new user and registers user.)<br/>
**Parameters:**
  - firstName
  - lastName
  - phoneNumber
  - subscriptionName

### Creating and using a local AzureSQL database  
AzureSQL, being a cloud-based service, it can be a hassle creating cloud environments for development and testing. Luckily, they have a docker image that can be used to create a local instance of AzureSQL.


The project during development makes use of a local database. To start the local database, run the following command:
```bash
make run 
```

What's going on under the hood?
- The command starts a local Azure SQL Server leveraging the [SQL Edge Image](https://hub.docker.com/r/microsoft/azure-sql-edge)
- The initialization script **dbInit.sh** is going to look for an active database connection. If found it will create the database and tables by restoring from a bacpac file. If for whatever reason the database connection is not found, it will simply time out.

Configure the local database connection string in the appsettings.json file for the service:
```json
{
    "AzureSQL": {
        "DataSource": "127.0.0.1,1433",
        "UserId": "SA",
        "Password": "StrongPassword!123",
        "InitialCatalog": "CheckInDB"
    }
}
```

Add your EntraId configuration and you're ready to go!

### Stop and delete all currently active containers
(Note: if you have other containers running, this command will stop and delete them.)

To clean up the local database, run the following command:
```bash
make drop 
```

## Misc.
[Design and architecture](Docs/Design.md)


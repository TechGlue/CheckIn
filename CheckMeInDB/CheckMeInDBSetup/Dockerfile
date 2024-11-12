# Fetch latest image for Azure SQL
FROM mcr.microsoft.com/azure-sql-edge

# Set environment variables
ENV ACCEPT_EULA=1
ENV MSSQL_SA_PASSWORD=StrongPassword!123
ENV MSSQL_PID=Developer
# Expose port 1433 for SQL Server
EXPOSE 1433

# copy the stored procedure file to the container
WORKDIR /src
COPY ["./create-temp-db.sh", "CheckMeInDBSetup/" ]
COPY ["./CheckMeIn_InitTables_SP.sql", "CheckMeInDBSetup/" ]
COPY ["./CheckMeIn_CreateDummyData_SP.sql", "CheckMeInDBSetup/" ]

WORKDIR "/src/CheckMeInDBSetup"
CMD ["/bin/bash", "./create-temp-db.sh"]

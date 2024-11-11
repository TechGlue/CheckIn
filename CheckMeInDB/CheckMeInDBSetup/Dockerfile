# Fetch latest image for Azure SQL
FROM mcr.microsoft.com/azure-sql-edge:latest

# Set environment variables
ENV MSSQL_SA_PASSWORD="StrongPassword!123"
ENV ACCEPT_EULA="Y"
ENV MSSQL_PID="Developer"

# Expose port 1433 for SQL Server
EXPOSE 1433

# copy the stored procedure file to the container
COPY ./create-temp-db.sh /usr/src/app/create-temp-db.sh
COPY ./CheckMeIn_InitTables_SP.sql /usr/src/app/CheckMeIn_InitTables_SP.sql
COPY ./CheckMeIn_CreateDummyData_SP.sql /usr/src/app/CheckMeIn_CreateDummyData_SP.sql

CMD ["/bin/bash", "/usr/src/app/create-temp-db.sh"]

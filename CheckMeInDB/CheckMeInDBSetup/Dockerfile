# Fetch latest image for Azure SQL
FROM mcr.microsoft.com/azure-sql-edge

# Set environment variables
ENV ACCEPT_EULA=1
ENV MSSQL_SA_PASSWORD=StrongPassword!123
ENV MSSQL_PID=Developer

# Expose the SQL Server port
EXPOSE 1433

# Copy the script to initialize the database
COPY ./CheckMeInDB/CheckMeIn_InitTables_SP.sql /app/CheckMeIn_InitTables_SP.sql

# Fetch latest image 
FROM mcr.microsoft.com/azure-sql-edge
# Set environment variables
ENV ACCEPT_EULA=1
ENV MSSQL_SA_PASSWORD=StrongPassword!123

# Using default
ENV MSSQL_PID=Developer

# switch to root to install sqlpackage
USER root 

# Download and install SqlPackage for restoring bacpac files
RUN wget https://aka.ms/sqlpackage-linux -O sqlpackage.zip \
    && unzip sqlpackage.zip -d /opt/sqlpackage \
    && chmod +x /opt/sqlpackage/sqlpackage \
    && ln -s /opt/sqlpackage/sqlpackage /usr/local/bin/sqlpackage \
    && rm sqlpackage.zip

USER mssql
EXPOSE 1433

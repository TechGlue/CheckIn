# Start the Azure SQL Edge Server in the background
/opt/mssql/bin/sqlservr &

while ! /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "StrongPassword!123"-Q "SELECT 1" > /dev/null; do
	echo "Waiting for Azure SQL Edge to start..."
	sleep 1
done

echo "Azure SQL Edge started."

# Create a new database based on the stored procedure
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "StrongPassword!123" -d master -i /usr/src/app/CheckMeIn_InitTables_SP.sql
 
wait

# Start the Azure SQL Edge Server in the background
/opt/mssql/bin/sqlservr &

# this path doesn't event exist in the container due to running on ARM
while ! /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "StrongPassword!123"-Q "SELECT 1" > /dev/null; do
	echo "Waiting for Azure SQL Edge to start..."
	sleep 1
done

echo "Azure SQL Edge started."

# Create a new database based on the stored procedure
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "StrongPassword!123" -d master -i ./CheckMeIn_InitTables_SP.sql
 
# Keep the process running until all previous commands are finished
wait

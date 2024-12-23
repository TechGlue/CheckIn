#!/bin/bash

# timeout tracker, if we exceed 5 tries, we will exit
timeoutTracker=1

# Loop until SQL Server is started and responsive 
while true; do
  OUTPUT=$(sqlcmd -S localhost -U sa -P "StrongPassword!123" -Q "SELECT 1")
  if [[ "$OUTPUT" == *"1"* && "$OUTPUT" == *"(1 row affected)"* ]]; then
    echo "Database is ready!"
    break
  elif [ $timeoutTracker -gt 5 ]; then
	echo "Timeout exceeded, check local db container health or connection configuration exiting..."
	exit 1 
  else
    echo "[$timeoutTracker] Waiting for database to be ready..."
	timeoutTracker=$((timeoutTracker + 1))
    sleep 5
  fi
done

# Restore the database from bacpac
echo "Restoring database..."
sqlpackage /TargetTrustServerCertificate:True /Action:Import /TargetDatabaseName:'CheckInDB' /SourceFile:'./CheckMeInDB/scripts/CheckMeInDB.bacpac' /TargetServerName:'127.0.0.1,1433' /TargetUser:'sa' /TargetPassword:'StrongPassword!123'

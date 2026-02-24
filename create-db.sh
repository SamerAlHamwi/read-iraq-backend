#!/bin/bash

# Check if a database name was provided
if [ -z "$1" ]
then
    echo "Usage: ./create-db.sh <DatabaseName>"
    exit 1
fi

DB_NAME=$1
SA_PASSWORD="YourSecurePassword123!"

echo "Creating database: $DB_NAME..."

# Execute SQL Command inside the docker container
docker exec -it $(docker ps -qf "name=db") /opt/mssql-tools/bin/sqlcmd \
   -S localhost -U sa -P $SA_PASSWORD \
   -Q "IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'$DB_NAME') CREATE DATABASE [$DB_NAME];"

if [ $? -eq 0 ]; then
    echo "Database '$DB_NAME' created successfully (or already existed)."
else
    echo "Failed to create database."
fi

#!/bin/bash

if [ -z "$1" ]; then
    echo "Usage: ./create-db.sh <DatabaseName>"
    exit 1
fi

DB_NAME=$1
SA_PASSWORD="YourSecurePassword123!"

# Find container
CONTAINER_ID=$(docker compose ps -q db)
if [ -z "$CONTAINER_ID" ]; then
    echo "Error: Database container not found."
    exit 1
fi

# Try to find sqlcmd in common paths
SQLCMD_PATH="/opt/mssql-tools18/bin/sqlcmd"
if ! docker exec $CONTAINER_ID test -f $SQLCMD_PATH; then
    SQLCMD_PATH="/opt/mssql-tools/bin/sqlcmd"
fi

echo "Using sqlcmd at: $SQLCMD_PATH"
echo "Creating database: $DB_NAME..."

# Execute with -C (Trust Server Certificate) which is required for tools v18+
docker exec -i $CONTAINER_ID $SQLCMD_PATH \
   -S localhost -U sa -P $SA_PASSWORD -C \
   -Q "IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'$DB_NAME') CREATE DATABASE [$DB_NAME];"

if [ $? -eq 0 ]; then
    echo "Success: Database '$DB_NAME' is ready."
else
    echo "Error: Failed to create database."
fi

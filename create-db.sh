#!/bin/bash

# Check if a database name was provided
if [ -z "$1" ]
then
    echo "Usage: ./create-db.sh <DatabaseName>"
    exit 1
fi

DB_NAME=$1
SA_PASSWORD="YourSecurePassword123!"

echo "Searching for the database container..."

# Try to find the container ID using docker compose
CONTAINER_ID=$(docker compose ps -q db)

if [ -z "$CONTAINER_ID" ]
then
    echo "Error: The database container ('db' service) is not running."
    echo "Please run 'docker compose up -d' first and wait a few seconds."
    exit 1
fi

echo "Creating database: $DB_NAME inside container $CONTAINER_ID..."

# Execute SQL Command
docker exec -i $CONTAINER_ID /opt/mssql-tools/bin/sqlcmd \
   -S localhost -U sa -P $SA_PASSWORD \
   -Q "IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'$DB_NAME') CREATE DATABASE [$DB_NAME];"

if [ $? -eq 0 ]; then
    echo "Success: Database '$DB_NAME' is ready."
else
    echo "Error: Failed to create database."
fi

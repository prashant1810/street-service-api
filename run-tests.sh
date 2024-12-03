#!/bin/bash

# Build the Docker image for the application.
docker build -t streetapi ./PTV.Services.StreetAPI/

# Start the containers defined in docker-compose.yml.
docker-compose up -d

# Run the integration tests.
dotnet test ./PTV.Services.IntegrationTests/PTV.Services.IntegrationTests.csproj

# Stop and remove the containers.
docker-compose down
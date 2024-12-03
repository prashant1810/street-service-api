# Street Geometry Service

A REST API for managing streets, their geometry, and capacity data, using .NET, EF Core, and PostgreSQL. It supports adding points to street geometries and is designed for scalability with Docker and Kubernetes.

## Features
- Create, delete, and update streets with their geometry and capacity.
- Add points to street geometry dynamically.
- Supports PostGIS for advanced geometry operations.
- Dockerized for local and production environments.
- Deployed using Kubernetes with 3 replicas for scalability.

## Getting Started

### Prerequisites
- .NET SDK 6.0+
- Docker and Docker Compose
- PostgreSQL

### Installation
Clone the repository:
   ```bash
   git clone https://github.com/prashant1810/street-service-api.git
   cd street-service-api
   docker compose up
   docker compose down
   
### Services Overview
1. Street API (ptv.services.streetapi):

- The main service for managing streets and their geometries.
- Listens on ports 5000 and 5001.

2. Database (street-db):

- A PostgreSQL database with PostGIS extensions for spatial data handling.
- Stores street data, including geometries.

3. Redis Cache (street-cache):

- A Redis instance used for caching street data.

4. Seq (street-seq):

- A structured log server for .NET applications.
- Accessible on port 5341 and 8081.

5. Loki (loki):

- A logging system compatible with Grafana.
- Used for collecting logs.

6. Promtail (promtail):

- A log collector that sends logs to Loki.

7. Grafana (grafana):

- A monitoring and visualization tool.
- Accessible on port 3000.

### How to Use
1. Prerequisites

- Ensure Docker and Docker Compose are installed.

2. Setup and Run

- Clone the repository and navigate to the project directory
- Start all services

3. Access the services:

- Services: http://localhost:5001/swagger/index.html
- Seq: http://localhost:8081/#/events?range=1d
- Loki: http://localhost:3100
- Grafana: http://localhost:3000

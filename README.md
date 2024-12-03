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
1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/street-geometry-service.git
   cd street-geometry-service

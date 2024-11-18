# CRM System Project
## There are some extra files in this repo because I was initially using sqlite for my database and switched to postgresql due to some weird errors I was getting in the docker container that I attributed to being Sqlite's fault. The only real important files are the docker-compose.yaml and the init.sql
## Prerequisites

- .NET 8 SDK (or higher) (only if you are running it with dotnet will you need this)
- Docker
- Visual Studio or VS Code (optional)

## Setup Instructions

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/crms2.git
2. Download Docker if you don't have it
3. `Docker-compose build` then `Docker-compose up -d`.
4. navigate to localhost:8080

   

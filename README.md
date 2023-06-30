# VPSMonitor
VPSMonitor - program that works with SSH.NET to monitoring and manage VPS resources, manage files, network configuration, and execute bash scripts and commands.

## :floppy_disk: Installation
Requirement for build:

- `.NET 7.0`
- `NodeJS`
- `PostgreSQL`
- `Docker`

*The following commands should be executed in a CMD, Bash or Powershell window. To do this, go to a folder on your computer, click in the folder path at the top and type CMD, then press enter.*

1. Clone the repository:
For this step you need Git installed, but you can just download the zip file instead by clicking the button at the top of this page ☝️

    `git clone https://github.com/hailrakeqq/VPSMonitor.git`
  

2. Navigate to the project directory:
*(Type this into your CMD window, you're aiming to navigate the CMD window to the repository you just downloaded)*

     `cd VPSMonitor`

3. Build with docker compose;
Run:

    `docker compose -f "docker-compose.yml" up -d --build`

After build you can get access to services by next URL's:
```
    `http://localhost:4200` - it's our frontend

    `https://localhost:5081` - it's our backend API

    `User ID={username};Password={password};Server=db;Port=5433;Database=vpsmonitor;Host=127.0.0.1;` - connection string to db
  ```
<br>For stop docker compose work you can use next command:

    `docker compose down`
  
  
### Features:
- [X] User authentication
- [X] Execution bash scripts and other commands
- [X] Obtaining information about status of the VPS
- [X] Linux users controls page
- [ ] File management
- [ ] Proccess management 

#### Technology stack:
- ASP.NET Core WebAPI
- Angular 
- SSH.NET
- PostgreSQL

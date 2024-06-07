# Overview
//Todo: (semi-)short description of the backend

# How to use
//Todo: short description on how to use the backend pages, maybe screenshot/video showcase

# Installation
//Todo: explanation of the installation of the backend

## Dependencies
//Todo: list big dependencies (used Frameworks etc.) with short description + how to install them, along with the steps for the right setup

# Functionality
//Todo: description of what the backend does, what it's used for

# Deployment
//Todo: docker stuff

# Development
//Todo: introduction on how the development of any new, fixed or refactored parts of the backend should be done

//Todo: link to documentation of the backend 

## Code Styling
//Todo: list code-styling conventions

## Conventions
//Todo: list general conventions for development

//Todo: helpful information and how-to's for development
## Fix testcontainers when using wsl instead of Docker Desktop
Go into your wsl terminal and navigate to ` /etc/docker/`. Create a **daemon.json** (or modify if already existing) File
with the following content:
```json
{
    "hosts": [
        "tcp://0.0.0.0:2375",
        "unix:///var/run/docker.sock"
    ]
}
```
Restart your wsl with `wsl --shutdown` in the Windows terminal and open your wsl again.
Start your docker-daemon with `sudo dockerd` or `sudo systemctl start docker`.

Verify that docker is now exposing the port `2375` with `netstat -nl | grep 2375`. 
This should produce the output `tcp6 0  0 ::: :::* LISTEN`.

In your Test `App.cs` you have to append 
```csharp
        .WithDockerEndpoint("tcp://localhost:2375")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
```
to your PostgreSqlBuilder so it now looks similar to this
```csharp
private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16.3")
        .WithDatabase("VoycarDb-Tests")
        .WithUsername("admin")
        .WithPassword("admin")
        .WithDockerEndpoint("tcp://localhost:2375")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
        .Build();
```
This should fix the issue with misconfigured docker exceptions for testcontainers.

You might need to restart docker and the docker-daemon several times until it works properly.
(See more information and help here [How to run integration tests using Testcontainers with WSL](https://medium.com/@NelsonBN/how-to-run-integration-tests-using-testcontainers-with-wsl-52c77a2acbbb) and [How to run tests with TestContainers in WSL2 without Docker Desktop
](https://gist.github.com/sz763/3b0a5909a03bf2c9c5a057d032bd98b7))
## Secrets
- Make a copy of `.env.example` and name it `.env` (will be Git-ignored)
- Set the names and values of your secret environment variables in there

## Logging
Logging based on the Serilog.AspNetCore NuGet package.

<table>
   <thead>
      <tr>
         <th>Level</th>
         <th>Usage</th>
      </tr>
   </thead>
   <tbody>
      <tr>
         <td>Verbose</td>
         <td>Verbose is the noisiest level, rarely (if ever) enabled for a production app.</td>
      </tr>
      <tr>
         <td>Debug</td>
         <td>Debug is used for internal system events that are not necessarily observable from the outside, but useful when determining how something happened.</td>
      </tr>
      <tr>
         <td>Information</td>
         <td>Information events describe things happening in the system that correspond to its responsibilities and functions.</td>
      </tr>
      <tr>
         <td>Warning</td>
         <td>When service is degraded, endangered, or maybe behaving outside its expected parameters, Warning-level events are used.</td>
      </tr>
      <tr>
         <td>Error</td>
         <td>When functionality is unavailable or expectations are broken, an Error event is used.</td>
      </tr>
      <tr>
         <td>Fatal</td>
         <td>The most critical level, Fatal events demand immediate attention.</td>
      </tr>
   </tbody>
</table>

## Migrations

### Command to create a migration: 
 `dotnet ef migrations add <Migration_Name>`

### Command to add migration to a database which is running in a docker container: 
`dotnet ef database update <Migration_Name> --connection "User ID=admin;Password=admin;Server=localhost;Port=5432;Database=VoycarDb"`

Migration will be added via localhost to the database but will be persistent due to the docker volume.

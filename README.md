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
 `dotnet ef migrations add "name"`

### Command to add migration to a database which is running in a docker container: 

`dotnet ef database update <Migration_Name> --connection "User ID=admin;Password=admin;Server=localhost;Port=5432;Database=VoycarDb"`

Migration will be added via localhost to the database but will be persistent due to the docker volume.


# Overview
//Todo: (semi-)short Desciption of the Backend
# Functionality
//Todo: what the backend does, what its used for,
# Installation
//Todo: explenation Installation of the backend + dependend Modules (Database etc.)
# Deployment 
//Docker build, Docker Compose --> start, stopp, restart backend
# Development
//Todo: introduction on how the development of any new, fixed or refactored parts of the backend
## Styling
//Todo: list styling conventions 
## Conventions
//Todo list general conventions for development

//Todo: helpfull informations and how-to's for Development
### Logging
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

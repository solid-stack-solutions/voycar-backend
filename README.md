<h1>
    <img src="https://raw.githubusercontent.com/solid-stack-solutions/voycar-frontend/main/static/logo-full-white.svg" height=25>
    Backend - Your modern carsharing solution
</h1>

> also see [Voycar Frontend](https://github.com/solid-stack-solutions/voycar-frontend)!

### Technology Overview
- [FastEndpoints](https://fast-endpoints.com/)
- [MailKit](https://github.com/jstedfast/MailKit) (sending verification mails)
- [BCrypt.Net-Next](https://www.nuget.org/packages/BCrypt.Net-Next) (password hashing)
- [Serilog.AspNetCore](https://github.com/serilog/serilog-aspnetcore) (logging)
- [PostgreSQL](https://www.postgresql.org/) database
   - [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
      - with [Npgsql provider for PostgreSQL](https://github.com/npgsql/efcore.pg)
- Automated testing with [xUnit](https://github.com/xunit/xunit) and...
   - [Fluent Assertions](https://fluentassertions.com/)
   - [FakeItEasy](https://fakeiteasy.github.io/) (mocking)
   - [Testcontainers](https://dotnet.testcontainers.org/) (specifically the PostgreSQL module)

# Installation and Usage
- Have [Docker](https://www.docker.com/) set up
- Get the source code, e.g. with...
```sh
git clone https://github.com/solid-stack-solutions/voycar-frontend
cd voycar-frontend
```
- Make a copy of `.env.example` and name it `.env` (will be Git-ignored)
- Set the names and values of your secret environment variables in there
- Run `docker compose up`

# Development

### Conventions
- Comments start with a space and a capital letter, e.g. `// This is a comment`
- Identifiers of properties are usually PascalCase, e.g. `MyProperty`
- Identifiers of dependency-injected properties are camelCase with a leading underscore, e.g. `_myProperty`

### Migrations
Database migrations can be generated automatically from the [EF Core `DbContext`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext?view=efcore-8.0). Creating migrations and updating the database with them is necessary every time an entity associated with the `DbContext` changes.

```sh
# Make sure you are in the right directory with the necessary commands available
cd Voycar.Api.Web
dotnet tool restore

# Create a migration
dotnet ef migrations add <Migration_Name>
# Update running database with created migration 
dotnet ef database update <Migration_Name> --connection "User ID=admin;Password=admin;Server=localhost;Port=5432;Database=VoycarDb"
```

If something doesn't work, it can help to delete old migrations ("Migrations" directory) and/or drop all the current tables in the database (but beware of the consequences).

### Logging
with [Serilog.AspNetCore](https://github.com/serilog/serilog-aspnetcore)

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

// Logging
global using Serilog;

// API Framework
global using FastEndpoints;
global using FastEndpoints.Swagger;
global using FastEndpoints.Security;

// API helper
global using FluentValidation;
global using Microsoft.AspNetCore.Http.HttpResults;

using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Tests")]

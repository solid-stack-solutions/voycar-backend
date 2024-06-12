// Logging
global using Serilog;

// API Framework
global using FastEndpoints;
global using FastEndpoints.Swagger;
global using FastEndpoints.Security;

// API helper
global using FluentValidation;
global using Microsoft.AspNetCore.Http.HttpResults;

// EF Core and helper
global using Microsoft.EntityFrameworkCore;
global using System.ComponentModel.DataAnnotations;



using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Tests")]

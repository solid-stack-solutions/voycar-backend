namespace Voycar.Api.Web.Generic;

using System.ComponentModel.DataAnnotations;

/// <summary>
///     Basic entity that every other entity should extend
/// </summary>
public class Entity
{
    [Key]
    public Guid Id { get; set; }
}

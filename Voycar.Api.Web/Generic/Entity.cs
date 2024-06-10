namespace Voycar.Api.Web.Generic;

using System.ComponentModel.DataAnnotations;

/// <summary>
///     basic entity that every other entity should extend
/// </summary>
public class Entity
{
    [Key]
    public Guid Id { get; set; }
}

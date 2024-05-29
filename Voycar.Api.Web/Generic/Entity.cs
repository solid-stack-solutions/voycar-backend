namespace Voycar.Api.Web.Entities;

using System.ComponentModel.DataAnnotations;

public abstract class Entity
{
    [Key]
    public Guid Id { get; set; }
}

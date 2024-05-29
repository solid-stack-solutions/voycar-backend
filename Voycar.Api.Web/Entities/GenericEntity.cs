namespace Voycar.Api.Web.Entities;

using System.ComponentModel.DataAnnotations;

public abstract class GenericEntity
{
    [Key]
    public Guid Id { get; set; }
}

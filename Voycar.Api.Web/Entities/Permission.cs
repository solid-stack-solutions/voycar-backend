namespace Voycar.Api.Web.Entities;

using System.ComponentModel.DataAnnotations;

public class Permission
{
    [Key, Required]
    public string Name { get; set; }
}

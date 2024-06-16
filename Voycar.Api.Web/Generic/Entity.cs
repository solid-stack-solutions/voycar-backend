namespace Voycar.Api.Web.Generic;

using System.ComponentModel.DataAnnotations;

/// <summary>
///     Basic entity that every other entity should extend
/// </summary>
public class Entity
{
    [Key]
    public Guid Id { get; set; }

    public override bool Equals(object? obj)
    {
        // Check if this ID is just the default ID
        if (obj == null || this.GetType() != obj.GetType() || this.Id.Equals(new Guid()))
        {
            return false;
        }

        var other = (Entity)obj;
        return this.Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.GetType().Name, this.Id);
    }
}
